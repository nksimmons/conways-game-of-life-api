namespace GameOfLife.Domain.Domain;

/// <summary>Resolves the well-known evolution rules by <see cref="RuleId"/>.</summary>
public static class LifeRules
{
    public static ILifeRule Resolve(RuleId id)
    {
        if (id == RuleId.Standard)
        {
            return StandardLifeRule.Instance;
        }

        throw new InvalidOperationException($"Unknown rule '{id}'.");
    }
}
