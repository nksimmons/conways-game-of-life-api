using GameOfLife.Domain.Domain;

namespace GameOfLife.Application.GetFinalState;

public sealed record GetFinalStateQuery(UniverseId Id, int IterationBudget) : IQuery<FinalStateView>;
