using GameOfLife.Domain.Common;
using GameOfLife.Domain.Domain;

namespace GameOfLife.Application.GetUniverse;

/// <summary>Handles retrieving the seed and metadata for a universe by id.</summary>
public sealed class GetUniverseQueryHandler(IUniverseRepository repository)
    : IQueryHandler<GetUniverseQuery, UniverseView>
{
    public async Task<Result<UniverseView>> HandleAsync(GetUniverseQuery query, CancellationToken ct)
    {
        var universe = await repository.FindAsync(query.Id, ct);
        if (universe is null) return Result<UniverseView>.NotFound();

        var view = new UniverseView(universe.Id, universe.Rule, universe.Topology, universe.CreatedAtUtc,
            universe.Seed);
        return Result<UniverseView>.Success(view);
    }
}