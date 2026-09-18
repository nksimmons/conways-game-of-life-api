using GameOfLife.Domain.Domain;
using GameOfLife.Domain.Observability;
using Microsoft.EntityFrameworkCore;

namespace GameOfLife.Infrastructure.Persistence;

/// <summary>
/// The only code in this repository that knows SQL exists. Translates between the domain
/// <see cref="Universe"/> and its <see cref="UniverseRecord"/> persistence shape.
/// </summary>
public sealed class EfUniverseRepository : IUniverseRepository
{
    private readonly GameOfLifeDbContext _dbContext;

    public EfUniverseRepository(GameOfLifeDbContext dbContext) => _dbContext = dbContext;

    public async Task<Universe?> FindAsync(UniverseId id, CancellationToken ct)
    {
        using var activity = GameOfLifeDiagnostics.ActivitySource.StartActivity("persistence.find-universe");

        // AsNoTracking: a universe is never updated after AddAsync (docs/design.md §7.1), so there is
        // nothing for the change tracker to track and no later SaveChanges that could need it. Skipping
        // it avoids the snapshot and identity-map bookkeeping EF would otherwise do for free.
        var record = await _dbContext.Universes
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id.Value, ct)
            .ConfigureAwait(false);

        if (record is null)
        {
            return null;
        }

        var seed = Pattern.FromPackedBytes(record.Width, record.Height, record.SeedPacked);
        return new Universe(
            new UniverseId(record.Id),
            seed,
            new RuleId(record.RuleId),
            new TopologyId(record.TopologyId),
            record.CreatedAtUtc);
    }

    public async Task AddAsync(Universe universe, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(universe);

        using var activity = GameOfLifeDiagnostics.ActivitySource.StartActivity("persistence.add-universe");

        var record = new UniverseRecord
        {
            Id = universe.Id.Value,
            Width = universe.Seed.Width,
            Height = universe.Seed.Height,
            RuleId = universe.Rule.Value,
            TopologyId = universe.Topology.Value,
            SeedPacked = universe.Seed.PackedBytes.ToArray(),
            CreatedAtUtc = universe.CreatedAtUtc,
        };

        _dbContext.Universes.Add(record);
        await _dbContext.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}
