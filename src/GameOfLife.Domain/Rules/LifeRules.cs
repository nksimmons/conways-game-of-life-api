namespace GameOfLife.Domain.Rules;

/// <summary>Resolves the well-known evolution rules by <see cref="RuleId" />.</summary>
public static class LifeRules
{
    private static readonly IReadOnlyDictionary<RuleId, ILifeRule> KnownRules = new Dictionary<RuleId, ILifeRule>
    {
        [RuleId.Standard] = StandardLifeRule.Instance,
        [RuleId.HighLife] = HighLifeRule.Instance,
        [RuleId.Seeds] = SeedsRule.Instance,
        [RuleId.LifeWithoutDeath] = LifeWithoutDeathRule.Instance,
        [RuleId.DayAndNight] = DayAndNightRule.Instance,
        [RuleId.Morley] = MorleyRule.Instance,
        [RuleId.TwoByTwo] = TwoByTwoRule.Instance,
        [RuleId.Diamoeba] = DiamoebaRule.Instance,
        [RuleId.Flock] = FlockRule.Instance,
        [RuleId.ThirtyFourLife] = ThirtyFourLifeRule.Instance
    };

    public static bool IsSupported(this RuleId id) => KnownRules.ContainsKey(id);

    public static ILifeRule Resolve(this RuleId id) => KnownRules.TryGetValue(id, out var rule)
        ? rule
        : throw new InvalidOperationException($"Unknown rule '{id}'.");
}
