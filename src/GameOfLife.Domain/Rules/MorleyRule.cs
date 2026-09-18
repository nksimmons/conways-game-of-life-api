namespace GameOfLife.Domain.Rules;

/// <summary>
///     The Morley (Move) rule, B368/S245: named after Stephen Morley, known for supporting complex mobile spaceships
///     and oscillators.
/// </summary>
public sealed class MorleyRule : ILifeRule
{
    public static readonly MorleyRule Instance = new();

    private MorleyRule()
    {
    }

    public RuleId Id => RuleId.Morley;

    public bool NextState(bool alive, int liveNeighbors) => (alive, liveNeighbors) switch
    {
        (false, 3 or 6 or 8) => true,
        (true, 2 or 4 or 5) => true,
        _ => false
    };
}
