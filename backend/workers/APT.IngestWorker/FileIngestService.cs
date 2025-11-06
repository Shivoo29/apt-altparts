
// backend/workers/APT.IngestWorker/FileIngestService.cs
using APT.Domain.Entities;
using APT.Infrastructure;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Globalization;

public class FileIngestService : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<FileIngestService> _log;
    private readonly string _watchDir;
    private readonly string _doneDir;
    private readonly string _errDir;

    public FileIngestService(IServiceProvider sp, ILogger<FileIngestService> log, IConfiguration cfg)
    {
        _sp = sp;
        _log = log;
        _watchDir = cfg.GetValue<string>("Ingest:WatchDir") ?? "/ingest";
        _doneDir = Path.Combine(_watchDir, "done");
        _errDir = Path.Combine(_watchDir, "err");
        Directory.CreateDirectory(_watchDir);
        Directory.CreateDirectory(_doneDir);
        Directory.CreateDirectory(_errDir);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.LogInformation("Watching {dir} for files...", _watchDir);

        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var file in Directory.GetFiles(_watchDir, "*.*").Where(f => f.EndsWith(".csv") || f.EndsWith(".xlsx")))
            {
                try
                {
                    await IngestFileAsync(file, stoppingToken);
                    File.Move(file, Path.Combine(_doneDir, Path.GetFileName(file)), true);
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "Failed ingest for {file}", file);
                    File.Move(file, Path.Combine(_errDir, Path.GetFileName(file)), true);
                }
            }
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task IngestFileAsync(string file, CancellationToken ct)
    {
        _log.LogInformation("Ingesting {file}", file);
        var records = Path.GetExtension(file).ToLowerInvariant() switch
        {
            ".csv" => ParseCsv(file),
            ".xlsx" => throw new NotImplementedException("XLSX parsing not implemented"), // Placeholder for ClosedXML
            _ => throw new InvalidOperationException($"Unsupported file type: {file}")
        };

        if (records.Count == 0) return;

        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Upsert logic
        var partNumbers = records.Select(r => r.PartNumber).Distinct().ToList();
        var existingParts = await db.Parts.Where(p => partNumbers.Contains(p.PartNumber)).ToDictionaryAsync(p => p.PartNumber, ct);

        foreach (var record in records)
        {
            if (!existingParts.TryGetValue(record.PartNumber, out var part))
            {
                part = new Part { PartNumber = record.PartNumber, Description = record.Description, Plant = record.Plant, IsAlternatePart = true };
                db.Parts.Add(part);
                existingParts[part.PartNumber] = part; // Add to dictionary to be found by subsequent records in the same file
            }

            if (!string.IsNullOrEmpty(record.PR_SourceId))
            {
                var pr = await db.ProblemReports.FirstOrDefaultAsync(p => p.SourceSystemId == record.PR_SourceId, ct);
                if (pr is null)
                {
                    pr = new ProblemReport
                    {
                        Part = part,
                        SourceSystemId = record.PR_SourceId,
                        ReasonCode = record.ReasonCode ?? "ALT_PART",
                        Status = Enum.TryParse<PRStatus>(record.Status, true, out var s) ? s : PRStatus.New,
                        OwnerUpn = record.Owner,
                        OpenedDate = DateTime.TryParse(record.OpenedDate, out var dt) ? dt : DateTime.UtcNow
                    };
                    db.ProblemReports.Add(pr);
                }
            }
        }

        await db.SaveChangesAsync(ct);
        _log.LogInformation("Ingest successful for {file}", file);
    }

    private List<IngestRecord> ParseCsv(string file)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { TrimOptions = TrimOptions.Trim, DetectDelimiter = true, MissingFieldFound = null };
        using var reader = new StreamReader(file);
        using var csv = new CsvReader(reader, config);
        return csv.GetRecords<IngestRecord>().ToList();
    }
}

public class IngestRecord
{
    public string PartNumber { get; set; } = default!;
    public string? Description { get; set; }
    public string? Plant { get; set; }
    public string? ReasonCode { get; set; }
    public string? PR_SourceId { get; set; }
    public string? Status { get; set; }
    public string? Owner { get; set; }
    public string? OpenedDate { get; set; }
}
