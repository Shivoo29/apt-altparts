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
        b.Entity<Part>()
            .HasIndex(p => p.PartNumber)
            .IsUnique();

        b.Entity<ProblemReport>()
            .HasOne(pr => pr.Part)
            .WithMany()
            .HasForeignKey(pr => pr.PartId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<PRStatusHistory>()
            .HasOne(h => h.ProblemReport)
            .WithMany(pr => pr.StatusHistory)
            .HasForeignKey(h => h.ProblemReportId);

        b.Entity<Shortage>()
            .HasOne(s => s.Part)
            .WithMany()
            .HasForeignKey(s => s.PartId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<Deviation>()
            .HasOne(d => d.Part)
            .WithMany()
            .HasForeignKey(d => d.PartId);

        b.Entity<Deviation>()
            .HasOne(d => d.ProblemReport)
            .WithMany()
            .HasForeignKey(d => d.ProblemReportId);

        // Reason code whitelist could be a separate table later.
    }
}