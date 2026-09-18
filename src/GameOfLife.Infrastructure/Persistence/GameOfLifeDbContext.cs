using Microsoft.EntityFrameworkCore;

namespace GameOfLife.Infrastructure.Persistence;

public sealed class GameOfLifeDbContext : DbContext
{
    public GameOfLifeDbContext(DbContextOptions<GameOfLifeDbContext> options)
        : base(options)
    {
    }

    internal DbSet<UniverseRecord> Universes => Set<UniverseRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UniverseRecord>(entity =>
        {
            entity.ToTable("Universes");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.RuleId).HasMaxLength(64).IsRequired();
            entity.Property(u => u.TopologyId).HasMaxLength(64).IsRequired();
            entity.Property(u => u.SeedPacked).IsRequired();
        });
    }
}
