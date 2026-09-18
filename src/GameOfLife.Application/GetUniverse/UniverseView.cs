using GameOfLife.Domain.Domain;

namespace GameOfLife.Application.GetUniverse;

/// <summary>A universe's identity, seed, and the rule and topology it evolves under.</summary>
public sealed record UniverseView(
    UniverseId Id,
    RuleId Rule,
    TopologyId Topology,
    DateTimeOffset CreatedAtUtc,
    Pattern Seed);