namespace GameOfLife.Domain.Rules;

/// <summary>
///     The Life without Death (Inkspot) rule, B3/S012345678: live cells never die and dead cells are born on
///     3 neighbours, forming intricate growing maze patterns.
/// </summary>
public sealed class LifeWithoutDeathRule : ILifeRule
{
    public static readonly LifeWithoutDeathRule Instance = new();

    private LifeWithoutDeathRule()
    {
    }

    public RuleId Id => RuleId.LifeWithoutDeath;

    public bool NextState(bool alive, int liveNeighbors) => (alive, liveNeighbors) switch
    {
        (false, 3) => true,
        (true, _) => true,
        _ => false
    };
}
