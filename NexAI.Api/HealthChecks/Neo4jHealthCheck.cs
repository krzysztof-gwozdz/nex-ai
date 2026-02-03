// ReSharper disable InconsistentNaming

using Microsoft.Extensions.Diagnostics.HealthChecks;
using NexAI.Neo4j;

namespace NexAI.Api.HealthChecks;

public class Neo4jHealthCheck(Neo4jDbClient client) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await client.Driver.VerifyConnectivityAsync();
            return HealthCheckResult.Healthy("Neo4j is reachable.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Neo4j is unreachable.", ex);
        }
    }
}
