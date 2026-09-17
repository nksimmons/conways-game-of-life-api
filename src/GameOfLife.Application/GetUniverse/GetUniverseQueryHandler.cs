using GameOfLife.Domain.Common;
using GameOfLife.Domain.Domain;

namespace GameOfLife.Application.GetUniverse;

public sealed class GetUniverseQueryHandler : IQueryHandler<GetUniverseQuery, UniverseView>
{
    private readonly IUniverseRepository _repository;

    public GetUniverseQueryHandler(IUniverseRepository repository) => _repository = repository;

    public async Task<Result<UniverseView>> HandleAsync(GetUniverseQuery query, CancellationToken ct)
    {
        var universe = await _repository.FindAsync(query.Id, ct).ConfigureAwait(false);
        if (universe is null)
        {
            return Result<UniverseView>.NotFound();
        }

        var view = new UniverseView(universe.Id, universe.Rule, universe.Topology, universe.CreatedAtUtc, universe.Seed);
        return Result<UniverseView>.Success(view);
    }
}
