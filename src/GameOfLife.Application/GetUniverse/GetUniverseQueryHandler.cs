using GameOfLife.Domain.Common;
using GameOfLife.Domain.Domain;

namespace GameOfLife.Application.GetUniverse;

public sealed class GetUniverseQueryHandler(IUniverseRepository repository)
    : IQueryHandler<GetUniverseQuery, UniverseView>
{
    public async Task<Result<UniverseView>> HandleAsync(GetUniverseQuery query, CancellationToken ct)
    {
        var universe = await repository.FindAsync(query.Id, ct).ConfigureAwait(false);
        if (universe is null)
        {
            return Result<UniverseView>.NotFound();
        }

        var view = new UniverseView(universe.Id, universe.Rule, universe.Topology, universe.CreatedAtUtc, universe.Seed);
        return Result<UniverseView>.Success(view);
    }
}
