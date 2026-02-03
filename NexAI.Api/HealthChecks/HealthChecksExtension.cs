using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace NexAI.Api.HealthChecks;

public static class HealthChecksExtension
{
    public static IServiceCollection AddHealthChecks(this IServiceCollection services, string applicationName)
    {
        services.AddHealthChecks()
            .AddCheck<LiveHealthCheck>("self", tags: ["live"])
            .AddCheck<MongoDbHealthCheck>("mongodb", tags: ["ready"])
            .AddCheck<Neo4jHealthCheck>("neo4j", tags: ["ready"])
            .AddCheck<QdrantHealthCheck>("qdrant", tags: ["ready"]);
        services
            .AddHealthChecksUI(settings =>
                settings.AddHealthCheckEndpoint(applicationName, "/health"))
            .AddInMemoryStorage();
        return services;
    }

    public static WebApplication UseHealthChecks(this WebApplication app)
    {
        app.MapHealthChecksUI();
        app.MapHealthChecks("/health",
            new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        app.MapHealthChecks("/health/live",
            new HealthCheckOptions
            {
                Predicate = reg => reg.Tags.Contains("live"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        app.MapHealthChecks("/health/ready",
            new HealthCheckOptions
            {
                Predicate = reg => reg.Tags.Contains("ready"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        return app;
    }
}