namespace GameOfLife.Domain.Domain;

/// <summary>Resolves the well-known evolution rules by <see cref="RuleId"/>.</summary>
public static class LifeRules
{
    // A branch, not a DI-registered dictionary or plugin registry: there is exactly one rule today,
    // and AGENTS.md §7 reserves ILifeRule as a named extensibility seam for a second one (e.g.
    // HighLife), not a general-purpose plugin mechanism to build in advance of that second case.
    public static ILifeRule Resolve(RuleId id)
    {
        if (id == RuleId.Standard)
        {
            return StandardLifeRule.Instance;
        }

        throw new InvalidOperationException($"Unknown rule '{id}'.");
    }
}
