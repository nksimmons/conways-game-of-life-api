namespace GameOfLife.Api.Contracts;

/// <summary>
/// The exercise's board representation: a rectangular 2D array of 0/1 values.
/// <para>
/// <c>Cells</c> is non-nullable deliberately. With nullable reference types enabled, MVC treats it as
/// required and rejects a null or absent <c>cells</c> during binding, so neither the validator nor the
/// controller carries a null case.
/// </para>
/// </summary>
public sealed record UploadBoardRequest(int[][] Cells);
