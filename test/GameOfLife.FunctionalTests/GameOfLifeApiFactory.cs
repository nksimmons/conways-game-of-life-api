using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace GameOfLife.FunctionalTests;

/// <summary>Boots the real Web host against a unique, throwaway SQLite file per test class instance.</summary>
public class GameOfLifeApiFactory : WebApplicationFactory<Program>
{
    public string DatabasePath { get; } = Path.Combine(Path.GetTempPath(), $"gameoflife-functional-{Guid.NewGuid():N}.db");

    protected virtual IReadOnlyDictionary<string, string?> ConfigurationOverrides { get; } = new Dictionary<string, string?>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            var settings = new Dictionary<string, string?>(ConfigurationOverrides)
            {
                ["ConnectionStrings:GameOfLife"] = $"Data Source={DatabasePath}",
            };
            config.AddInMemoryCollection(settings);
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing && File.Exists(DatabasePath))
        {
            File.Delete(DatabasePath);
        }
    }
}
