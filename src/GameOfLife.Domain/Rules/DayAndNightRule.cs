namespace GameOfLife.Domain.Rules;

/// <summary>
///     The Day and Night rule, B3678/S34678: symmetric under on/off state inversion, where patterns evolve
///     identically in an inverted field of live cells.
/// </summary>
public sealed class DayAndNightRule : ILifeRule
{
    public static readonly DayAndNightRule Instance = new();

    private DayAndNightRule()
    {
    }

    public RuleId Id => RuleId.DayAndNight;

    public bool NextState(bool alive, int liveNeighbors) => (alive, liveNeighbors) switch
    {
        (false, 3 or 6 or 7 or 8) => true,
        (true, 3 or 4 or 6 or 7 or 8) => true,
        _ => false
    };
}
