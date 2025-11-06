namespace APT.Domain.Entities;

public class PRStatusHistory
{
    public int Id { get; set; }
    public int ProblemReportId { get; set; }
    public ProblemReport ProblemReport { get; set; } = default!;
    public PRStatus? OldStatus { get; set; }
    public PRStatus NewStatus { get; set; }
    public string ChangedByUpn { get; set; } = default!;
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public string? Comments { get; set; }
}