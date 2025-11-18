using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NexAI.Config;

namespace NexAI.ServiceBus;

public static class ServiceBusExtensions
{
    public static IServiceCollection AddServiceBus(this IServiceCollection services) =>
        services
            .AddSingleton<RabbitMQClient>();
}