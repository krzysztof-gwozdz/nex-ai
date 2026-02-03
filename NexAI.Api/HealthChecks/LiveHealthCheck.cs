using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NexAI.Api.HealthChecks;

public class LiveHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        Task.FromResult(HealthCheckResult.Healthy("Application is running."));
}
