namespace GameOfLife.Domain.Rules;

/// <summary>
///     The HighLife rule, B36/S23: identical to standard Life with the addition of birth on 6 live neighbours,
///     which supports the famous self-replicator pattern.
/// </summary>
public sealed class HighLifeRule : ILifeRule
{
    public static readonly HighLifeRule Instance = new();

    private HighLifeRule()
    {
    }

    public RuleId Id => RuleId.HighLife;

    public bool NextState(bool alive, int liveNeighbors) => (alive, liveNeighbors) switch
    {
        (false, 3 or 6) => true,
        (true, 2 or 3) => true,
        _ => false
    };
}
