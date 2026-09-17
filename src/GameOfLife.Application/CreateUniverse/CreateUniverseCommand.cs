using GameOfLife.Domain.Domain;

namespace GameOfLife.Application.CreateUniverse;

/// <summary>Saves a seed as a new universe. The id is minted by the caller so it is known before persistence.</summary>
public sealed record CreateUniverseCommand(UniverseId UniverseId, Pattern Seed) : ICommand;
