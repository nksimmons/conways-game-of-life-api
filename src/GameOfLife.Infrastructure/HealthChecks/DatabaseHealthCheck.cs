using GameOfLife.Infrastructure.Persistence;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GameOfLife.Infrastructure.HealthChecks;

/// <summary>Readiness check: is the universe store reachable. Never used for liveness.</summary>
public sealed class DatabaseHealthCheck(GameOfLifeDbContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            var canConnect = await dbContext.Database.CanConnectAsync(ct).ConfigureAwait(false);
            return canConnect
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy("The universe store is not reachable.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("The universe store health check failed.", ex);
        }
    }
}
