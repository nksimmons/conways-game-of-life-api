using GameOfLife.Domain.Domain;

namespace GameOfLife.Application.GetUniverse;

/// <summary>Fetches the aggregate root and seed pattern for a given universe id.</summary>
public sealed record GetUniverseQuery(UniverseId Id) : IQuery<UniverseView>;