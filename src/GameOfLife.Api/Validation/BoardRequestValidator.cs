using GameOfLife.Api.Options;

namespace GameOfLife.Api.Validation;

/// <summary>Boundary validation for the upload and generation-index requests. Domain guard clauses run separately, inside Core.</summary>
public static class BoardRequestValidator
{
    public const string MissingCellsError = "cells must be a non-empty 2D array.";

    public static IReadOnlyList<string> ValidateCells(int[][] cells, GameOfLifeOptions options)
    {
        var errors = new List<string>();

        if (cells.Length == 0)
        {
            errors.Add(MissingCellsError);
            return errors;
        }

        var height = cells.Length;
        var firstRow = cells[0];
        var width = firstRow?.Length ?? 0;

        if (width == 0)
        {
            errors.Add("cells rows must be non-empty.");
            return errors;
        }

        var rectangular = true;
        foreach (var row in cells)
        {
            if (row is null || row.Length != width)
            {
                rectangular = false;
                break;
            }
        }

        if (!rectangular)
        {
            errors.Add("cells must be rectangular; every row must have the same length.");
        }

        var validValues = true;
        foreach (var row in cells)
        {
            if (row is null)
            {
                continue;
            }

            foreach (var value in row)
            {
                if (value is not (0 or 1))
                {
                    validValues = false;
                    break;
                }
            }

            if (!validValues)
            {
                break;
            }
        }

        if (!validValues)
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

    public static IReadOnlyList<string> ValidateGeneration(int n, GameOfLifeOptions options)
    {
        if (n < 0 || n > options.MaxGenerationsAhead)
        {
            return new[] { $"n must be between 0 and {options.MaxGenerationsAhead}." };
        }

        return Array.Empty<string>();
    }
}
