namespace GameOfLife.Domain.Rules;

/// <summary>
///     The Diamoeba rule, B35678/S5678: forms large diamond-shaped amoeba-like oscillating boundaries.
/// </summary>
public sealed class DiamoebaRule : ILifeRule
{
    public static readonly DiamoebaRule Instance = new();

    private DiamoebaRule()
    {
    }

    public RuleId Id => RuleId.Diamoeba;

    public bool NextState(bool alive, int liveNeighbors) => (alive, liveNeighbors) switch
    {
        (false, 3 or 5 or 6 or 7 or 8) => true,
        (true, 5 or 6 or 7 or 8) => true,
        _ => false
    };
}
