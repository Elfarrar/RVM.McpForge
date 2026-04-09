using Microsoft.Extensions.Diagnostics.HealthChecks;
using RVM.McpForge.Infrastructure.Data;

namespace RVM.McpForge.API.Health;

public class DatabaseHealthCheck(McpForgeDbContext db) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            await db.Database.CanConnectAsync(ct);
            return HealthCheckResult.Healthy("Database connection is healthy.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database connection failed.", ex);
        }
    }
}
