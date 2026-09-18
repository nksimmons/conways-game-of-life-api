using Microsoft.EntityFrameworkCore;

namespace GameOfLife.Infrastructure.Persistence;

/// <summary>Entity Framework database context for persisting universe records.</summary>
public sealed class GameOfLifeDbContext(DbContextOptions<GameOfLifeDbContext> options) : DbContext(options)
{
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