using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GameOfLife.Api.Swagger;

/// <summary>
/// Supplies a sample glider as the Swagger UI request body for board creation. An operation filter,
/// not an attribute on <see cref="Contracts.UploadBoardRequest"/>, because Swashbuckle's built-in
/// example support is schema-level: an attribute there would apply the same example to every action
/// using that DTO. Filtering on the operation keeps it attached to the one operation it documents.
/// </summary>
public sealed class GliderExampleOperationFilter : IOperationFilter
{
    private const int GridSize = 15;

    // The standard glider, offset from the origin so the example shows it mid-flight rather than
    // already touching the boundary, where a bounded topology would change what it does.
    private static readonly (int Row, int Col)[] LiveCells = [(5, 6), (6, 7), (7, 5), (7, 6), (7, 7)];

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.HttpMethod != HttpMethods.Post ||
            context.ApiDescription.RelativePath != "api/v1/boards" ||
            operation.RequestBody?.Content.TryGetValue("application/json", out var mediaType) != true ||
            mediaType is null)
        {
            return;
        }

        var rows = new OpenApiArray();
        for (var row = 0; row < GridSize; row++)
        {
            var cells = new OpenApiArray();
            for (var col = 0; col < GridSize; col++)
            {
                cells.Add(new OpenApiInteger(LiveCells.Contains((row, col)) ? 1 : 0));
            }

            rows.Add(cells);
        }

        mediaType.Example = new OpenApiObject { ["cells"] = rows };
    }
}
