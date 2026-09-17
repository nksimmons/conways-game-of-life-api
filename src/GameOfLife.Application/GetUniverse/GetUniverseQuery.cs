using GameOfLife.Domain.Domain;

namespace GameOfLife.Application.GetUniverse;

public sealed record GetUniverseQuery(UniverseId Id) : IQuery<UniverseView>;
