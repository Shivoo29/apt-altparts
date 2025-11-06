
// backend/src/APT.Application/Services/ProblemReportService.cs
using AutoMapper;
using AutoMapper.QueryableExtensions;
using APT.Application.DTOs;
using APT.Domain.Entities;
using APT.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace APT.Application.Services;

public class ProblemReportService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public ProblemReportService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ProblemReportDetailDto?> GetAsync(int id) =>
        await _db.ProblemReports
            .AsNoTracking()
            .Where(p => p.Id == id)
            .ProjectTo<ProblemReportDetailDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

    public async Task<List<ProblemReportListItemDto>> ListAsync(string? reasonCode = null, PRStatus? status = null, int skip = 0, int take = 100)
    {
        var q = _db.ProblemReports.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(reasonCode)) q = q.Where(p => p.ReasonCode == reasonCode);
        if (status.HasValue) q = q.Where(p => p.Status == status.Value);
        return await q.OrderByDescending(p => p.OpenedDate)
            .Skip(skip).Take(take)
            .ProjectTo<ProblemReportListItemDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<(bool success, string? error)> UpdateStatusAsync(int id, PRStatus newStatus, string changedByUpn, string? comments = null)
    {
        var pr = await _db.ProblemReports.FirstOrDefaultAsync(p => p.Id == id);
        if (pr is null) return (false, "Problem Report not found.");

        var allowed = new Dictionary<PRStatus, PRStatus[]>
        {
            [PRStatus.New] = new[] { PRStatus.InAnalysis, PRStatus.Closed },
            [PRStatus.InAnalysis] = new[] { PRStatus.AwaitingDeviation, PRStatus.Approved, PRStatus.Closed },
            [PRStatus.AwaitingDeviation] = new[] { PRStatus.Approved, PRStatus.Closed },
            [PRStatus.Approved] = new[] { PRStatus.Implemented, PRStatus.Closed },
            [PRStatus.Implemented] = new[] { PRStatus.Closed }
        };

        if (pr.Status != newStatus && (!allowed.TryGetValue(pr.Status, out var next) || !next.Contains(newStatus)))
            return (false, $"Invalid status transition from {pr.Status} to {newStatus}.");

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
        return (true, null);
    }
}
