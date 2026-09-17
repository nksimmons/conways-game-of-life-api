using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace GameOfLife.Domain.Observability;

/// <summary>
/// The tracing/metrics vocabulary for this bounded context. Uses only BCL diagnostics primitives
/// (System.Diagnostics.DiagnosticSource, part of the shared framework since .NET 5), so this can live
/// in Core and be used by both Application and Infrastructure without either depending on an APM vendor
/// package or on each other. Web wires an OpenTelemetry SDK subscription onto this source and meter;
/// see Program.cs.
/// </summary>
public static class GameOfLifeDiagnostics
{
    public const string Name = "GameOfLife";

    public static readonly ActivitySource ActivitySource = new(Name);

    private static readonly Meter Meter = new(Name);

    public static readonly Counter<long> GenerationsComputed =
        Meter.CreateCounter<long>("gameoflife.generations_computed", unit: "{generation}", description: "Generations computed across all requests.");

    public static readonly Histogram<double> EvolutionDurationMs =
        Meter.CreateHistogram<double>("gameoflife.evolution_duration", unit: "ms", description: "Wall-clock time spent computing generations for a single request.");

    public static readonly Counter<long> ConvergenceOutcomes =
        Meter.CreateCounter<long>("gameoflife.convergence_outcomes", description: "Final-state requests, tagged by whether a cycle was found within budget.");
}
