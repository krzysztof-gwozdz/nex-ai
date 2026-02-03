using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using NexAI.MongoDb;

namespace NexAI.Api.HealthChecks;

public class MongoDbHealthCheck(MongoDbClient client) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new BsonDocument("ping", 1);
            await client.Database.RunCommandAsync<BsonDocument>(command, cancellationToken: cancellationToken);
            return HealthCheckResult.Healthy("MongoDB is reachable.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("MongoDB is unreachable.", ex);
        }
    }
}
