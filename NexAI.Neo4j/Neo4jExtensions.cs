// ReSharper disable InconsistentNaming

using Microsoft.Extensions.DependencyInjection;

namespace NexAI.Neo4j;

public static class Neo4jExtensions
{
    public static IServiceCollection AddNeo4j(this IServiceCollection services) =>
        services
            .AddScoped<Neo4jDbClient>();
}