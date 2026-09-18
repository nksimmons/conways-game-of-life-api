using GameOfLife.Domain.Domain;
using GameOfLife.Domain.Observability;
using GameOfLife.Domain.Rules;
using Microsoft.EntityFrameworkCore;

namespace GameOfLife.Infrastructure.Persistence;

/// <summary>
///     The only code in this repository that knows SQL exists. Translates between the domain
///     <see cref="Universe" /> and its <see cref="UniverseRecord" /> persistence shape.
/// </summary>
public sealed class EfUniverseRepository(GameOfLifeDbContext dbContext) : IUniverseRepository
{
    public async Task<Universe?> FindAsync(UniverseId id, CancellationToken ct)
    {
        using var activity = GameOfLifeDiagnostics.ActivitySource.StartActivity("persistence.find-universe");

        // AsNoTracking: a universe is never updated after AddAsync (README.md §7.1), so there is
        // nothing for the change tracker to track and no later SaveChanges that could need it. Skipping
        // it avoids the snapshot and identity-map bookkeeping EF would otherwise do for free.
        var record = await dbContext.Universes
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id.Value, ct);

        if (record is null) return null;

        // Named, because width and height are adjacent ints: a swap compiles, packs to the same byte
        // count, and silently transposes every non-square board read back from storage.
        var seed = Pattern.FromPackedBytes(width: record.Width, height: record.Height, record.SeedPacked);
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
            SeedPacked = [.. universe.Seed.PackedBytes],
            CreatedAtUtc = universe.CreatedAtUtc
        };

        // Synchronous Add: entity tracking is CPU-bound in memory. AddAsync exists only for
        // special value generators (such as Hi-Lo sequences) and would incur unnecessary ValueTask overhead here.
        dbContext.Universes.Add(record);
        await dbContext.SaveChangesAsync(ct);
    }
}
