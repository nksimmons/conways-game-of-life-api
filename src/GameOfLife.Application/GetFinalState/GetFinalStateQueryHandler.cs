using System.Diagnostics;
using GameOfLife.Domain.Common;
using GameOfLife.Domain.Domain;
using GameOfLife.Domain.Observability;

namespace GameOfLife.Application.GetFinalState;

public sealed class GetFinalStateQueryHandler : IQueryHandler<GetFinalStateQuery, FinalStateView>
{
    private readonly IUniverseRepository _repository;

    public GetFinalStateQueryHandler(IUniverseRepository repository) => _repository = repository;

    public async Task<Result<FinalStateView>> HandleAsync(GetFinalStateQuery query, CancellationToken ct)
    {
        var universe = await _repository.FindAsync(query.Id, ct).ConfigureAwait(false);
        if (universe is null)
        {
            return Result<FinalStateView>.NotFound();
        }

        using var activity = GameOfLifeDiagnostics.ActivitySource.StartActivity("evolution.determine-fate");
        activity?.SetTag("gameoflife.iteration_budget", query.IterationBudget);

        var stopwatch = Stopwatch.StartNew();
        var fate = universe.DetermineFate(query.IterationBudget, ct);
        stopwatch.Stop();

        GameOfLifeDiagnostics.EvolutionDurationMs.Record(stopwatch.Elapsed.TotalMilliseconds);
        GameOfLifeDiagnostics.ConvergenceOutcomes.Add(1, new KeyValuePair<string, object?>("converged", fate is Fate.Stabilized));

        var view = fate switch
        {
            // IterationsExamined is AtGeneration + Period, not just AtGeneration: the search had to walk
            // one full period past the cycle start to observe the repeat, so that sum is what it examined.
            Fate.Stabilized stabilized => new FinalStateView(
                universe.Id,
                IterationsExamined: stabilized.AtGeneration + stabilized.Period,
                new FinalStateView.Cycle(
                    stabilized.AtGeneration,
                    stabilized.Period,
                    universe.GenerationAt(stabilized.AtGeneration, ct))),
            Fate.Undetermined undetermined => new FinalStateView(universe.Id, undetermined.GenerationsExamined, Stabilized: null),
            _ => throw new InvalidOperationException($"Unknown fate type '{fate.GetType()}'."),
        };

        GameOfLifeDiagnostics.GenerationsComputed.Add(view.IterationsExamined);

        return Result<FinalStateView>.Success(view);
    }
}
