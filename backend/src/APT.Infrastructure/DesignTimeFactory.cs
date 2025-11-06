// backend/src/APT.Infrastructure/DesignTimeFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace APT.Infrastructure
{
    public class DesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=localhost,1433;Database=APT;User Id=sa;Password=Strong@Passw0rd;TrustServerCertificate=True;")
                .Options;
            return new AppDbContext(options);
        }
    }
}
