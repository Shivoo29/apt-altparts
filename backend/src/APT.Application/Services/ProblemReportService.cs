// backend/src/APT.Application/Services/ProblemReportService.cs
using APT.Domain.Entities;
using APT.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace APT.Application.Services;

public class ProblemReportService
{
    private readonly AppDbContext _db;

    public ProblemReportService(AppDbContext db) => _db = db;

    public async Task<ProblemReport?> GetAsync(int id) =>
        await _db.ProblemReports.Include(p => p.Part).Include(p => p.StatusHistory).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<List<ProblemReport>> ListAsync(string? reasonCode = null, PRStatus? status = null)
    {
        var q = _db.ProblemReports.Include(p => p.Part).AsQueryable();
        if (!string.IsNullOrWhiteSpace(reasonCode)) q = q.Where(p => p.ReasonCode == reasonCode);
        if (status.HasValue) q = q.Where(p => p.Status == status.Value);
        return await q.OrderByDescending(p => p.OpenedDate).Take(1000).ToListAsync();
    }

    public async Task<bool> UpdateStatusAsync(int id, PRStatus newStatus, string changedByUpn, string? comments = null)
    {
        var pr = await _db.ProblemReports.FirstOrDefaultAsync(p => p.Id == id);
        if (pr is null) return false;

        // Basic transition rules
        var allowed = new Dictionary<PRStatus, PRStatus[]>
        {
            [PRStatus.New] = new[] { PRStatus.InAnalysis },
            [PRStatus.InAnalysis] = new[] { PRStatus.AwaitingDeviation, PRStatus.Approved },
            [PRStatus.AwaitingDeviation] = new[] { PRStatus.Approved },
            [PRStatus.Approved] = new[] { PRStatus.Implemented, PRStatus.Closed },
            [PRStatus.Implemented] = new[] { PRStatus.Closed }
        };

        if (!allowed.TryGetValue(pr.Status, out var next) || !next.Contains(newStatus) && newStatus != pr.Status)
            throw new InvalidOperationException($"Invalid transition {pr.Status} -> {newStatus}");

        var old = pr.Status;
        pr.Status = newStatus;
        if (newStatus == PRStatus.Closed) pr.ClosedDate = DateTime.UtcNow;

        _db.PRStatusHistory.Add(new PRStatusHistory
        {
            ProblemReportId = pr.Id,
            OldStatus = old,
            NewStatus = newStatus,
            ChangedByUpn = changedByUpn,
            Comments = comments,
            ChangedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return true;
    }
}