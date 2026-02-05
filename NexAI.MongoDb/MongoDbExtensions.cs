using Microsoft.Extensions.DependencyInjection;

namespace NexAI.MongoDb;

public static class MongoDbExtensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services) =>
        services
            .AddScoped<MongoDbClient>();
}