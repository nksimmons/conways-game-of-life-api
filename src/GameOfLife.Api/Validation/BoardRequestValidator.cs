using GameOfLife.Api.Options;

namespace GameOfLife.Api.Validation;

/// <summary>Boundary validation for the upload and generation-index requests. Domain guard clauses run separately, inside Core.</summary>
public static class BoardRequestValidator
{
    public const string MissingCellsError = "cells must be a non-empty 2D array.";

    public static IReadOnlyList<string> ValidateCells(int[][] cells, GameOfLifeOptions options)
    {
        if (cells.Length == 0)
        {
            return new[] { MissingCellsError };
        }

        var height = cells.Length;
        var width = cells[0]?.Length ?? 0;
        if (width == 0)
        {
            return new[] { "cells rows must be non-empty." };
        }

        var errors = new List<string>();

        if (Array.Exists(cells, row => row is null || row.Length != width))
        {
            errors.Add("cells must be rectangular; every row must have the same length.");
        }

        if (Array.Exists(cells, row => row is not null && Array.Exists(row, value => value is not (0 or 1))))
        {
            errors.Add("cell values must be 0 or 1.");
        }

        if (width > options.MaxWidth || height > options.MaxHeight)
        {
            errors.Add($"board dimensions must not exceed {options.MaxWidth}x{options.MaxHeight}.");
        }

        if ((long)width * height > options.MaxCells)
        {
            errors.Add($"board must not contain more than {options.MaxCells} cells.");
        }

        return errors;
    }

    public static IReadOnlyList<string> ValidateGeneration(int n, GameOfLifeOptions options) =>
        n < 0 || n > options.MaxGenerationsAhead
            ? new[] { $"n must be between 0 and {options.MaxGenerationsAhead}." }
            : Array.Empty<string>();
}
