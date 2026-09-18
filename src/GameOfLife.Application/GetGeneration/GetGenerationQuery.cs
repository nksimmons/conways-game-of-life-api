using GameOfLife.Domain.Domain;

namespace GameOfLife.Application.GetGeneration;

/// <summary>Requests the pattern at generation <paramref name="Generation" />, computed from the seed.</summary>
public sealed record GetGenerationQuery(UniverseId Id, int Generation) : IQuery<PatternView>;