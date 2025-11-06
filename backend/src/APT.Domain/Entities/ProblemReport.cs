namespace APT.Domain.Entities;

public enum PRStatus { New, InAnalysis, AwaitingDeviation, Approved, Implemented, Closed }

public class ProblemReport
{
    public int Id { get; set; }
    public int PartId { get; set; }
    public Part Part { get; set; } = default!;
    public string? SourceSystemId { get; set; }
    public string ReasonCode { get; set; } = "ALT_PART";
    public PRStatus Status { get; set; } = PRStatus.New;
    public string? Priority { get; set; } // Low/Med/High
    public string? OwnerUpn { get; set; }
    public DateTime OpenedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ClosedDate { get; set; }
    public DateTime? SlaDue { get; set; }
    public ICollection<PRStatusHistory> StatusHistory { get; set; } = new List<PRStatusHistory>();
}