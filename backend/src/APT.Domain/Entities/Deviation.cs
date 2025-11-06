namespace APT.Domain.Entities;

public class Deviation
{
    public int Id { get; set; }
    public int PartId { get; set; }
    public Part Part { get; set; } = default!;
    public int? ProblemReportId { get; set; }
    public ProblemReport? ProblemReport { get; set; }
    public string Type { get; set; } = default!; // e.g. Form/Fit/Function
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string? ApprovedByUpn { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}