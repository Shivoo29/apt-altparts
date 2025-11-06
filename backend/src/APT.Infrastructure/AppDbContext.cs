
// backend/src/APT.Infrastructure/AppDbContext.cs
using APT.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace APT.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Part> Parts => Set<Part>();
    public DbSet<ProblemReport> ProblemReports => Set<ProblemReport>();
    public DbSet<PRStatusHistory> PRStatusHistory => Set<PRStatusHistory>();
    public DbSet<Shortage> Shortages => Set<Shortage>();
    public DbSet<Deviation> Deviations => Set<Deviation>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // Part
        b.Entity<Part>(e =>
        {
            e.HasIndex(p => p.PartNumber).IsUnique();
            e.Property(p => p.PartNumber).HasMaxLength(50);
            e.Property(p => p.Plant).HasMaxLength(10);
        });

        // ProblemReport
        b.Entity<ProblemReport>(e =>
        {
            e.HasIndex(pr => new { pr.PartId, pr.ReasonCode, pr.Status });
            e.Property(pr => pr.ReasonCode).HasMaxLength(20);
            e.HasOne(pr => pr.Part).WithMany().HasForeignKey(pr => pr.PartId).OnDelete(DeleteBehavior.Restrict);
        });

        // PRStatusHistory
        b.Entity<PRStatusHistory>(e =>
        {
            e.HasIndex(h => h.ProblemReportId);
            e.HasOne(h => h.ProblemReport).WithMany(pr => pr.StatusHistory).HasForeignKey(h => h.ProblemReportId);
        });

        // Shortage
        b.Entity<Shortage>(e =>
        {
            e.HasIndex(s => s.PartId);
            e.HasOne(s => s.Part).WithMany().HasForeignKey(s => s.PartId).OnDelete(DeleteBehavior.Cascade);
        });

        // Deviation
        b.Entity<Deviation>(e =>
        {
            e.HasIndex(d => d.PartId);
            e.HasOne(d => d.Part).WithMany().HasForeignKey(d => d.PartId);
            e.HasOne(d => d.ProblemReport).WithMany().HasForeignKey(d => d.ProblemReportId).IsRequired(false);
        });

        SeedData(b);
    }

    private void SeedData(ModelBuilder b)
    {
        b.Entity<Part>().HasData(
            new Part { Id = 1, PartNumber = "PN-1234", Description = "A very common part", Plant = "P1" },
            new Part { Id = 2, PartNumber = "PN-5678", Description = "A less common part", Plant = "P2" }
        );

        b.Entity<ProblemReport>().HasData(
            new ProblemReport { Id = 1, PartId = 1, ReasonCode = "ALT_PART", Status = PRStatus.New, OwnerUpn = "shivam.jha@example.com", OpenedDate = DateTime.UtcNow.AddDays(-10) },
            new ProblemReport { Id = 2, PartId = 2, ReasonCode = "ALT_PART", Status = PRStatus.InAnalysis, OwnerUpn = "shivam.jha@example.com", OpenedDate = DateTime.UtcNow.AddDays(-5) },
            new ProblemReport { Id = 3, PartId = 1, ReasonCode = "DEVIATION", Status = PRStatus.Approved, OwnerUpn = "shivam.jha@example.com", OpenedDate = DateTime.UtcNow.AddDays(-1) }
        );

        b.Entity<PRStatusHistory>().HasData(
            new PRStatusHistory { Id = 1, ProblemReportId = 1, NewStatus = PRStatus.New, ChangedByUpn = "system", ChangedAt = DateTime.UtcNow.AddDays(-10) },
            new PRStatusHistory { Id = 2, ProblemReportId = 2, NewStatus = PRStatus.New, ChangedByUpn = "system", ChangedAt = DateTime.UtcNow.AddDays(-5) },
            new PRStatusHistory { Id = 3, ProblemReportId = 2, OldStatus = PRStatus.New, NewStatus = PRStatus.InAnalysis, ChangedByUpn = "shivam.jha@example.com", ChangedAt = DateTime.UtcNow.AddDays(-4), Comments = "Starting analysis" },
            new PRStatusHistory { Id = 4, ProblemReportId = 3, NewStatus = PRStatus.New, ChangedByUpn = "system", ChangedAt = DateTime.UtcNow.AddDays(-1) },
            new PRStatusHistory { Id = 5, ProblemReportId = 3, OldStatus = PRStatus.New, NewStatus = PRStatus.Approved, ChangedByUpn = "shivam.jha@example.com", ChangedAt = DateTime.UtcNow, Comments = "Approved" }
        );
    }
}
