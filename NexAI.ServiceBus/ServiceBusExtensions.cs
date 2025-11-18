using Microsoft.Extensions.Hosting;
using NexAI.Config;

namespace NexAI.ServiceBus;

public static class ServiceBusExtensions
{
    public static IHostBuilder UseServiceBus(this IHostBuilder hostBuilder, string endpointName) =>
        hostBuilder.UseNServiceBus(context =>
        {
            var options = new Options(context.Configuration).Get<RabbitMQOptions>();
            
            var endpointConfiguration = new EndpointConfiguration(endpointName);
            endpointConfiguration.UseSerialization<SystemJsonSerializer>();
            endpointConfiguration.EnableInstallers();
            
            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            transport.UseConventionalRoutingTopology(QueueType.Quorum);
            transport.ConnectionString(options.ConnectionString);
            
            return endpointConfiguration;
        });
}