namespace GameOfLife.Domain.Rules;

/// <summary>Identifies which <see cref="ILifeRule" /> a universe evolves under.</summary>
public readonly record struct RuleId(string Value)
{
    /// <summary>
    ///     Conway's standard Game of Life rule (B3/S23): birth on 3 live neighbors, survival on 2 or 3 live neighbors.
    /// </summary>
    public static readonly RuleId Standard = new("B3/S23");

    /// <summary>
    ///     HighLife rule (B36/S23): birth on 3 or 6 live neighbors, survival on 2 or 3 live neighbors.
    /// </summary>
    public static readonly RuleId HighLife = new("B36/S23");

    /// <summary>
    ///     Seeds rule (B2/S): all live cells die; birth on 2 live neighbors.
    /// </summary>
    public static readonly RuleId Seeds = new("B2/S");

    /// <summary>
    ///     Life without Death (Inkspot) rule (B3/S012345678): live cells never die; birth on 3 live neighbors.
    /// </summary>
    public static readonly RuleId LifeWithoutDeath = new("B3/S012345678");

    /// <summary>
    ///     Day and Night rule (B3678/S34678): symmetric under on/off state inversion.
    /// </summary>
    public static readonly RuleId DayAndNight = new("B3678/S34678");

    /// <summary>
    ///     Morley / Move rule (B368/S245): named after Stephen Morley, known for mobile spaceships and oscillators.
    /// </summary>
    public static readonly RuleId Morley = new("B368/S245");

    /// <summary>
    ///     2x2 rule (B36/S125): patterns composed of 2x2 blocks evolve cleanly.
    /// </summary>
    public static readonly RuleId TwoByTwo = new("B36/S125");

    /// <summary>
    ///     Diamoeba rule (B35678/S5678): forms diamond-shaped oscillating boundaries.
    /// </summary>
    public static readonly RuleId Diamoeba = new("B35678/S5678");

    /// <summary>
    ///     Flock rule (B3/S12): modified Conway Life with survival on 1 or 2 live neighbors.
    /// </summary>
    public static readonly RuleId Flock = new("B3/S12");

    /// <summary>
    ///     34 Life rule (B34/S34): birth on 3 or 4 live neighbors, survival on 3 or 4 live neighbors.
    /// </summary>
    public static readonly RuleId ThirtyFourLife = new("B34/S34");

    public override string ToString() => Value;
}