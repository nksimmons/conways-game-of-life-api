using GameOfLife.Domain.Domain;

namespace GameOfLife.Application.GetGeneration;

/// <summary>The evaluated pattern of a universe at a specific generation.</summary>
public sealed record PatternView(UniverseId UniverseId, int Generation, Pattern Pattern);