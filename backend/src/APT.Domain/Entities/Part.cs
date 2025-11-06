// backend/src/APT.Domain/Entities/Part.cs
namespace APT.Domain.Entities;

public class Part
{
    public int Id { get; set; }
    public string PartNumber { get; set; } = default!;
    public string? Description { get; set; }
    public string? Plant { get; set; }
    public int? SupplierId { get; set; }
    public bool IsAlternatePart { get; set; }
    public string? LifecycleState { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// backend/src/APT.Domain/Entities/ProblemReport.cs
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

// backend/src/APT.Domain/Entities/PRStatusHistory.cs
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

// backend/src/APT.Domain/Entities/Shortage.cs
namespace APT.Domain.Entities;

public class Shortage
{
    public int Id { get; set; }
    public int PartId { get; set; }
    public Part Part { get; set; } = default!;
    public DateTime Date { get; set; }
    public int QtyRequired { get; set; }
    public int QtyOnHand { get; set; }
    public int CoverageDays { get; set; }
    public string? DemandRef { get; set; }
}

// backend/src/APT.Domain/Entities/Deviation.cs
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