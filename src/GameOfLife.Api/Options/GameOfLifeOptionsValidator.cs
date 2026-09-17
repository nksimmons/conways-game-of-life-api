using Microsoft.Extensions.Options;

namespace GameOfLife.Api.Options;

/// <summary>Fails at boot rather than on the first request if the configuration is nonsensical.</summary>
public sealed class GameOfLifeOptionsValidator : IValidateOptions<GameOfLifeOptions>
{
    public ValidateOptionsResult Validate(string? name, GameOfLifeOptions options)
    {
        var errors = new List<string>();

        if (options.MaxWidth <= 0)
        {
            errors.Add($"{nameof(options.MaxWidth)} must be positive.");
        }

        if (options.MaxHeight <= 0)
        {
            errors.Add($"{nameof(options.MaxHeight)} must be positive.");
        }

        if (options.MaxCells <= 0)
        {
            errors.Add($"{nameof(options.MaxCells)} must be positive.");
        }

        if (options.MaxGenerationsAhead < 0)
        {
            errors.Add($"{nameof(options.MaxGenerationsAhead)} cannot be negative.");
        }

        if (options.FinalStateIterationBudget <= 0)
        {
            errors.Add($"{nameof(options.FinalStateIterationBudget)} must be positive.");
        }

        if (options.MaxConcurrentEvaluations < 0)
        {
            errors.Add($"{nameof(options.MaxConcurrentEvaluations)} cannot be negative.");
        }

        return errors.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(errors);
    }
}
