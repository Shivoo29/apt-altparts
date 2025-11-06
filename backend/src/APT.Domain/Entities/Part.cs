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