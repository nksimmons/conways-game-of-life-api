using GameOfLife.Domain.Domain;
using GameOfLife.Domain.Rules;

namespace GameOfLife.Application.CreateUniverse;

/// <summary>Saves a seed as a new universe. The id is minted by the caller so it is known before persistence.</summary>
public sealed record CreateUniverseCommand(
    UniverseId UniverseId,
    Pattern Seed,
    RuleId? Rule = null,
    TopologyId? Topology = null) : ICommand;