using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace GameOfLife.FunctionalTests;

/// <summary>
/// The 422 contract from docs/design.md §6.3, exercised by shrinking the server-configured budget
/// rather than by a pattern that outlasts the default. On a bounded grid the published methuselahs
/// (Acorn included) settle well inside 5000 generations, so the default budget cannot be exceeded by a
/// legitimately sized input; a small budget is the honest way to reach this path.
/// </summary>
public sealed class FinalStateNonConvergenceTests : IClassFixture<FinalStateNonConvergenceTests.LowBudgetFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _client;

    public FinalStateNonConvergenceTests(LowBudgetFactory factory) => _client = factory.CreateClient();

    public sealed class LowBudgetFactory : GameOfLifeApiFactory
    {
        protected override IReadOnlyDictionary<string, string?> ConfigurationOverrides { get; } =
            new Dictionary<string, string?> { ["GameOfLife:FinalStateIterationBudget"] = "5" };
    }

    [Fact]
    public async Task GetFinalState_returns_422_problem_details_when_the_budget_is_exhausted()
    {
        var rPentomino = new
        {
            cells = new[]
            {
                new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new[] { 0, 0, 0, 0, 1, 1, 0, 0, 0, 0 },
                new[] { 0, 0, 0, 1, 1, 0, 0, 0, 0, 0 },
                new[] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
                new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            },
        };

        var created = await _client.PostAsJsonAsync("/api/v1/boards", rPentomino);
        var location = created.Headers.Location!;

        var response = await _client.GetAsync($"{location}/final");

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(JsonOptions);
        Assert.NotNull(problem);
        Assert.Equal(422, problem!.Status);
        Assert.Contains("Examined 5 generations", problem.Detail);
    }
}
