namespace GameOfLife.Api.Contracts;

/// <summary>The exercise's board representation: a rectangular 2D array of 0/1 values.</summary>
public sealed record UploadBoardRequest(int[][]? Cells);
