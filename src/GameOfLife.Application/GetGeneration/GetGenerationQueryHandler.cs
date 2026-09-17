using System.Diagnostics;
using GameOfLife.Domain.Common;
using GameOfLife.Domain.Domain;
using GameOfLife.Domain.Observability;

namespace GameOfLife.Application.GetGeneration;

public sealed class GetGenerationQueryHandler : IQueryHandler<GetGenerationQuery, PatternView>
{
    private readonly IUniverseRepository _repository;

    public GetGenerationQueryHandler(IUniverseRepository repository) => _repository = repository;

    public async Task<Result<PatternView>> HandleAsync(GetGenerationQuery query, CancellationToken ct)
    {
        var universe = await _repository.FindAsync(query.Id, ct).ConfigureAwait(false);
        if (universe is null)
        {
            return Result<PatternView>.NotFound();
        }

        using var activity = GameOfLifeDiagnostics.ActivitySource.StartActivity("evolution.generate");
        activity?.SetTag("gameoflife.generation", query.Generation);

        var stopwatch = Stopwatch.StartNew();
        var pattern = universe.GenerationAt(query.Generation, ct);
        stopwatch.Stop();

        GameOfLifeDiagnostics.GenerationsComputed.Add(query.Generation);
        GameOfLifeDiagnostics.EvolutionDurationMs.Record(stopwatch.Elapsed.TotalMilliseconds);

        return Result<PatternView>.Success(new PatternView(universe.Id, query.Generation, pattern));
    }
}
