using GameOfLife.Domain.Rules;

namespace GameOfLife.Domain.Domain;

/// <summary>
///     An immutable seed together with the rule and topology that make it replayable. Generations are
///     computed on demand from the seed; nothing is ever advanced, mutated, or checkpointed.
/// </summary>
public sealed class Universe
{
    private readonly ILifeRule _rule;
    private readonly ITopology _topology;

    public Universe(UniverseId id, Pattern seed, RuleId rule, TopologyId topology, DateTimeOffset createdAtUtc)
    {
        ArgumentNullException.ThrowIfNull(seed);

        Id = id;
        Seed = seed;
        Rule = rule;
        Topology = topology;
        CreatedAtUtc = createdAtUtc;
        _rule = rule.Resolve();
        _topology = topology.Resolve();
    }

    public UniverseId Id { get; }

    public Pattern Seed { get; }

    public RuleId Rule { get; }

    public TopologyId Topology { get; }

    public DateTimeOffset CreatedAtUtc { get; }

    /// <summary>
    ///     Computes generation <paramref name="n" /> by applying the rule <paramref name="n" /> times, starting from the
    ///     seed.
    /// </summary>
    public Pattern GenerationAt(int n, CancellationToken ct)
    {
        if (n < 0) throw new ArgumentOutOfRangeException(nameof(n), n, "Generation index cannot be negative.");

        var current = Seed;
        foreach (var _ in Enumerable.Range(0, n))
        {
            ct.ThrowIfCancellationRequested();
            current = current.NextGeneration(_rule, _topology, ct);
        }

        return current;
    }

    /// <summary>
    ///     Generates forward, hashing each generation, until a cycle is detected or the iteration budget
    ///     is exhausted. A hash hit is a candidate only: the candidate generation is recomputed from the
    ///     seed and compared in full before a cycle is declared, so a hash collision cannot produce a
    ///     wrong answer.
    /// </summary>
    public Fate DetermineFate(int iterationBudget, CancellationToken ct)
    {
        if (iterationBudget < 0)
            throw new ArgumentOutOfRangeException(nameof(iterationBudget), iterationBudget,
                "Iteration budget cannot be negative.");

        var seen = new Dictionary<StateHash, int>();
        var current = Seed;
        long generationsComputed = 0;

        foreach (var generation in Enumerable.Range(0, Math.Max(1, iterationBudget)))
        {
            ct.ThrowIfCancellationRequested();

            var hash = current.ComputeHash();
            if (seen.TryGetValue(hash, out var candidateGeneration))
            {
                var candidatePattern = GenerationAt(candidateGeneration, ct);
                generationsComputed += candidateGeneration;
                if (candidatePattern.Equals(current))
                    return new Fate.Stabilized(candidateGeneration, generation - candidateGeneration,
                        candidatePattern, generationsComputed);
            }

            seen[hash] = generation;

            if (generation + 1 >= iterationBudget) break;

            current = current.NextGeneration(_rule, _topology, ct);
            generationsComputed++;
        }

        return new Fate.Undetermined(iterationBudget, generationsComputed);
    }
}
