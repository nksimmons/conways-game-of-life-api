namespace GameOfLife.Domain.Rules;

/// <summary>
///     The Flock rule, B3/S12: modified Conway Life where cells survive with 1 or 2 neighbours, forming flock-like
///     structures.
/// </summary>
public sealed class FlockRule : ILifeRule
{
    public static readonly FlockRule Instance = new();

    private FlockRule()
    {
    }

    public RuleId Id => RuleId.Flock;

    public bool NextState(bool alive, int liveNeighbors) => (alive, liveNeighbors) switch
    {
        (false, 3) => true,
        (true, 1 or 2) => true,
        _ => false
    };
}
