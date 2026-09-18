using FluentValidation;
using GameOfLife.Api.Contracts;
using GameOfLife.Api.Options;
using Microsoft.Extensions.Options;

namespace GameOfLife.Api.Validation;

/// <summary>
///     Boundary validation for the board upload. Domain guard clauses in <c>Pattern.FromRows</c> enforce
///     the same invariants independently; these exist to turn a bad request into a 400 rather than an
///     exception, and the caps among them are denial-of-service controls (docs/design.md §10.1).
/// </summary>
public sealed class UploadBoardRequestValidator : AbstractValidator<UploadBoardRequest>
{
    public UploadBoardRequestValidator(IOptions<GameOfLifeOptions> options)
    {
        var caps = options.Value;

        // Cascade.Stop because the later rules index into the grid: without a non-empty rectangle
        // there is nothing coherent to report on, and reporting "rows must be non-empty" alongside
        // "must not exceed 256x256" for the same empty array is noise rather than help.
        //
        // NotNull is kept as a runtime guard even though Cells is non-nullable. MVC rejects a null or
        // absent cells before this runs, but only because non-nullable reference types are implicitly
        // required; that inference can be switched off in MvcOptions, and the rules below index into
        // the array.
        RuleFor(request => request.Cells)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("cells must be a non-empty 2D array.")
            .Must(cells => cells.Length > 0)
            .WithMessage("cells must be a non-empty 2D array.")
            .Must(cells => cells.All(row => row is not null))
            .WithMessage("cells rows must not be null.")
            .Must(cells => (cells[0]?.Length ?? 0) > 0)
            .WithMessage("cells rows must be non-empty.")
            .DependentRules(() =>
            {
                RuleFor(request => request.Cells)
                    .Must(BeRectangular)
                    .WithMessage("cells must be rectangular; every row must have the same length.");

                RuleFor(request => request.Cells)
                    .Must(HoldOnlyBinaryValues)
                    .WithMessage("cell values must be 0 or 1.");

                RuleFor(request => request.Cells)
                    .Must(cells => cells[0].Length <= caps.MaxWidth && cells.Length <= caps.MaxHeight)
                    .WithMessage($"board dimensions must not exceed {caps.MaxWidth}x{caps.MaxHeight}.");

                RuleFor(request => request.Cells)
                    .Must(cells => (long)cells[0].Length * cells.Length <= caps.MaxCells)
                    .WithMessage($"board must not contain more than {caps.MaxCells} cells.");
            });
    }

    private static bool BeRectangular(int[][] cells) =>
        cells.All(row => row.Length == cells[0].Length);

    private static bool HoldOnlyBinaryValues(int[][] cells) =>
        cells.All(row => row.All(value => value is 0 or 1));
}
