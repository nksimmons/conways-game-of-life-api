using GameOfLife.Domain.Domain;

namespace GameOfLife.Application.GetFinalState;

/// <summary>Reads the final stabilized state of a universe within a given iteration budget.</summary>
public sealed record GetFinalStateQuery(UniverseId Id, int IterationBudget) : IQuery<FinalStateView>;