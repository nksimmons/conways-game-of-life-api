namespace GameOfLife.Domain.Rules;

/// <summary>
///     The Seeds rule, B2/S: all live cells die at every generation, but dead cells are born on exactly 2 neighbours,
///     producing chaotic expanding growth patterns.
/// </summary>
public sealed class SeedsRule : ILifeRule
{
    public static readonly SeedsRule Instance = new();

    private SeedsRule()
    {
    }

    public RuleId Id => RuleId.Seeds;

    public bool NextState(bool alive, int liveNeighbors) => (alive, liveNeighbors) switch
    {
        (false, 2) => true,
        _ => false
    };
}
