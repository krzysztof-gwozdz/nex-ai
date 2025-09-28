using Microsoft.Extensions.Hosting;
using NexAI.RabbitMQ;
using Spectre.Console;

namespace NexAI.DataProcessor.ConsumerServices;

public class RabbitMQConsumerService<TMessage>(RabbitMQConsumer<TMessage> rabbitMQConsumer) : BackgroundService
{
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await rabbitMQConsumer.Init(cancellationToken);
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await rabbitMQConsumer.Execute(cancellationToken);
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
    }
}