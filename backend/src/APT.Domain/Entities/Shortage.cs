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