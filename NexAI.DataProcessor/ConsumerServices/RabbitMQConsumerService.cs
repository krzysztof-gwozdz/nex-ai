using Microsoft.Extensions.Hosting;
using NexAI.RabbitMQ;
using Spectre.Console;

namespace NexAI.DataProcessor.ConsumerServices;

public class RabbitMQConsumerService<TMessage>(RabbitMQConsumer<TMessage> rabbitMQConsumer) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await rabbitMQConsumer.ExecuteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
    }
}