namespace GameOfLife.Domain.Rules;

/// <summary>
///     The 2x2 rule, B36/S125: patterns composed of 2x2 blocks evolve cleanly and group together.
/// </summary>
public sealed class TwoByTwoRule : ILifeRule
{
    public static readonly TwoByTwoRule Instance = new();

    private TwoByTwoRule()
    {
    }

    public RuleId Id => RuleId.TwoByTwo;

    public bool NextState(bool alive, int liveNeighbors) => (alive, liveNeighbors) switch
    {
        (false, 3 or 6) => true,
        (true, 1 or 2 or 5) => true,
        _ => false
    };
}
