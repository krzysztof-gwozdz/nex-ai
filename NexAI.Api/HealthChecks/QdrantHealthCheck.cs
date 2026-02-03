using Microsoft.Extensions.Diagnostics.HealthChecks;
using NexAI.Qdrant;

namespace NexAI.Api.HealthChecks;

public class QdrantHealthCheck(QdrantDbClient client) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await client.ListCollectionsAsync(cancellationToken);
            return HealthCheckResult.Healthy("Qdrant is reachable.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Qdrant is unreachable.", ex);
        }
    }
}
