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

    public FileIngestService(IServiceProvider sp, ILogger<FileIngestService> log, IConfiguration cfg)
    {
        _sp = sp; _log = log;
        _watchDir = cfg.GetValue<string>("Ingest:WatchDir") ?? "/ingest";
        Directory.CreateDirectory(_watchDir);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.LogInformation("Watching {dir} for CSV files...", _watchDir);

        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var file in Directory.GetFiles(_watchDir, "*.csv"))
            {
                try { await IngestFileAsync(file, stoppingToken); File.Move(file, file + ".done", true); }
                catch (Exception ex) { _log.LogError(ex, "Failed ingest {file}", file); File.Move(file, file + ".err", true); }
            }
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private async Task IngestFileAsync(string file, CancellationToken ct)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        _log.LogInformation("Ingesting {file}", file);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { TrimOptions = TrimOptions.Trim, DetectDelimiter = true };
        using var reader = new StreamReader(file);
        using var csv = new CsvReader(reader, config);

        var records = csv.GetRecords<dynamic>().ToList();

        foreach (var r in records)
        {
            // Example expected columns: PartNumber, Description, Plant, ReasonCode, PR_SourceId, Status, Owner, OpenedDate
            var dict = (IDictionary<string, object>)r;
            string partNumber = dict["PartNumber"]?.ToString() ?? throw new("PartNumber required");

            var part = await db.Parts.FirstOrDefaultAsync(p => p.PartNumber == partNumber, ct);
            if (part is null)
            {
                part = new Part { PartNumber = partNumber, Description = dict.TryGetValue("Description", out var d) ? d?.ToString() : null, Plant = dict.TryGetValue("Plant", out var pl) ? pl?.ToString() : null, IsAlternatePart = true };
                db.Parts.Add(part);
                await db.SaveChangesAsync(ct);
            }

            if (dict.TryGetValue("PR_SourceId", out var srcObj) && srcObj is not null)
            {
                var src = srcObj.ToString();
                var pr = await db.ProblemReports.FirstOrDefaultAsync(p => p.SourceSystemId == src, ct);
                if (pr is null)
                {
                    pr = new ProblemReport
                    {
                        PartId = part.Id,
                        SourceSystemId = src,
                        ReasonCode = dict.TryGetValue("ReasonCode", out var rc) ? rc?.ToString() ?? "ALT_PART" : "ALT_PART",
                        Status = Enum.TryParse<PRStatus>(dict.TryGetValue("Status", out var st) ? st?.ToString() : "New", true, out var s) ? s : PRStatus.New,
                        OwnerUpn = dict.TryGetValue("Owner", out var own) ? own?.ToString() : null,
                        OpenedDate = DateTime.TryParse(dict.TryGetValue("OpenedDate", out var od) ? od?.ToString() : null, out var dt) ? dt : DateTime.UtcNow
                    };
                    db.ProblemReports.Add(pr);
                }
            }
        }

        await db.SaveChangesAsync(ct);
        _log.LogInformation("Ingest OK: {file}", file);
    }
}