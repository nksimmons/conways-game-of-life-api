using GameOfLife.Domain.Domain;

namespace GameOfLife.Application.GetGeneration;

public sealed record PatternView(UniverseId UniverseId, int Generation, Pattern Pattern);
