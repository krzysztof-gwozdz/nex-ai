using Microsoft.Extensions.DependencyInjection;

namespace NexAI.Qdrant;

public static class QdrantExtensions
{
    public static IServiceCollection AddQdrant(this IServiceCollection services) =>
        services
            .AddScoped<QdrantDbClient>();
}