namespace GameOfLife.Domain.Rules;

/// <summary>
///     The standard Game of Life rule, B3/S23: a live cell survives on 2 or 3 neighbours; a dead cell is born on
///     exactly 3.
/// </summary>
public sealed class StandardLifeRule : ILifeRule
{
    // See BoundedTopology.Instance for why this is a singleton.
    public static readonly StandardLifeRule Instance = new();

    private StandardLifeRule()
    {
    }

    public RuleId Id => RuleId.Standard;

    public bool NextState(bool alive, int liveNeighbors) => (alive, liveNeighbors) switch
    {
        (false, 3) => true,
        (true, 2 or 3) => true,
        _ => false
    };
}
