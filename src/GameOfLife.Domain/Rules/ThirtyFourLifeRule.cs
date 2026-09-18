namespace GameOfLife.Domain.Rules;

/// <summary>
///     The 34 Life rule, B34/S34: birth on 3 or 4 live neighbours, survival on 3 or 4 live neighbours.
/// </summary>
public sealed class ThirtyFourLifeRule : ILifeRule
{
    public static readonly ThirtyFourLifeRule Instance = new();

    private ThirtyFourLifeRule()
    {
    }

    public RuleId Id => RuleId.ThirtyFourLife;

    public bool NextState(bool alive, int liveNeighbors) => (alive, liveNeighbors) switch
    {
        (false, 3 or 4) => true,
        (true, 3 or 4) => true,
        _ => false
    };
}
