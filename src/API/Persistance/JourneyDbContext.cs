using Microsoft.EntityFrameworkCore;

namespace API.Persistance;

public class JourneyDbContext : DbContext
{
    public JourneyDbContext(DbContextOptions<JourneyDbContext> options)
    : base(options) { JourneyEntries = Set<JourneyEntry>(); }

    public DbSet<JourneyEntry> JourneyEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JourneyEntry>().HasKey(e => e.Id);
    }
}

public record JourneyEntry(Guid Id, string Username, DateTime CreatedAt);
