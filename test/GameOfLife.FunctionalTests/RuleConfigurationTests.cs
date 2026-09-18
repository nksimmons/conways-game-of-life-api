using System.Net.Http.Json;
using GameOfLife.Api.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace GameOfLife.FunctionalTests;

public sealed class RuleConfigurationTests
{
    [Theory]
    [InlineData("B3/S23", 0)]
    [InlineData("B36/S23", 1)]
    public async Task Create_persists_the_configured_rule_and_uses_it_for_evolution(string rule, int expectedCenter)
    {
        using var factory = new RuleFactory(rule);
        using var client = factory.CreateClient();
        var created = await client.PostAsJsonAsync("/api/v1/boards", new
        {
            cells = new[] { new[] { 1, 1, 1 }, new[] { 1, 0, 1 }, new[] { 0, 1, 0 } }
        });
        created.EnsureSuccessStatusCode();
        Assert.NotNull(created.Headers.Location);

        var saved = await client.GetFromJsonAsync<BoardResponse>(created.Headers.Location);
        var next = await client.GetFromJsonAsync<GenerationResponse>($"{created.Headers.Location}/next");

        Assert.NotNull(saved);
        Assert.Equal(rule, saved.Rule);
        Assert.NotNull(next);
        Assert.Equal(expectedCenter, next.Cells[1][1]);

        using var changedDefault = factory.WithWebHostBuilder(builder =>
            builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(
                new Dictionary<string, string?> { ["GameOfLife:DefaultRule"] = "B2/S" })));
        using var changedClient = changedDefault.CreateClient();
        var replayed = await changedClient.GetFromJsonAsync<GenerationResponse>($"{created.Headers.Location}/next");
        Assert.NotNull(replayed);
        Assert.Equal(next.Cells, replayed.Cells);
    }

    [Theory]
    [InlineData("unknown")]
    [InlineData("B2/S0")]
    [InlineData("")]
    public void Unsupported_default_rule_fails_at_startup(string rule)
    {
        using var factory = new RuleFactory(rule);

        var error = Assert.Throws<OptionsValidationException>(() => factory.CreateClient());

        Assert.Contains("DefaultRule", error.Message, StringComparison.Ordinal);
    }

    private sealed class RuleFactory(string rule) : GameOfLifeApiFactory
    {
        protected override IReadOnlyDictionary<string, string?> ConfigurationOverrides { get; } =
            new Dictionary<string, string?> { ["GameOfLife:DefaultRule"] = rule };
    }
}
