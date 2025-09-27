using Microsoft.Extensions.DependencyInjection;

namespace NexAI.RabbitMQ;

public static class RabbitMQExtensions
{
    public static IServiceCollection AddRabbitMQ(this IServiceCollection services) =>
        services
            .AddSingleton<RabbitMQClient>();
}