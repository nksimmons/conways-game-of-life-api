namespace GameOfLife.Domain.Domain;

/// <summary>The evolution rule: whether a cell lives, dies, or is born given its current state and live-neighbour count.</summary>
public interface ILifeRule
{
    RuleId Id { get; }

    bool NextState(bool alive, int liveNeighbors);
}
