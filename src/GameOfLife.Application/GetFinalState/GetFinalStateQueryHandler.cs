using System.Diagnostics;
using GameOfLife.Domain.Common;
using GameOfLife.Domain.Domain;
using GameOfLife.Domain.Observability;

namespace GameOfLife.Application.GetFinalState;

/// <summary>Computes the fate of a universe, recording diagnostic metrics across the search and verification.</summary>
public sealed class GetFinalStateQueryHandler(IUniverseRepository repository)
    : IQueryHandler<GetFinalStateQuery, FinalStateView>
{
    public async Task<Result<FinalStateView>> HandleAsync(GetFinalStateQuery query, CancellationToken ct)
    {
        var universe = await repository.FindAsync(query.Id, ct);
        if (universe is null) return Result<FinalStateView>.NotFound();

        using var activity = GameOfLifeDiagnostics.ActivitySource.StartActivity("evolution.determine-fate");
        activity?.SetTag("gameoflife.iteration_budget", query.IterationBudget);

        var stopwatch = Stopwatch.StartNew();
        var fate = universe.DetermineFate(query.IterationBudget, ct);
        stopwatch.Stop();

        GameOfLifeDiagnostics.EvolutionDurationMs.Record(stopwatch.Elapsed.TotalMilliseconds);
        GameOfLifeDiagnostics.ConvergenceOutcomes.Add(1,
            new KeyValuePair<string, object?>("converged", fate is Fate.Stabilized));

        var view = fate switch
        {
            // IterationsExamined is AtGeneration + Period, not just AtGeneration: the search had to walk
            // one full period past the cycle start to observe the repeat, so that sum is what it examined.
            Fate.Stabilized stabilized => new FinalStateView(
                universe.Id,
                stabilized.AtGeneration + stabilized.Period,
                new FinalStateView.Cycle(
                    stabilized.AtGeneration,
                    stabilized.Period,
                    stabilized.Pattern)),
            Fate.Undetermined undetermined => new FinalStateView(universe.Id, undetermined.GenerationsExamined, null),
            _ => throw new InvalidOperationException($"Unknown fate type '{fate.GetType()}'.")
        };

        GameOfLifeDiagnostics.GenerationsComputed.Add(fate.GenerationsComputed);

        return Result<FinalStateView>.Success(view);
    }
}
