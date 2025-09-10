using Microsoft.Extensions.Hosting;
using NexAI.Config;
using NexAI.DataProcessor.Zendesk;
using NexAI.RabbitMQ;
using NexAI.Zendesk;
using Spectre.Console;

namespace NexAI.DataProcessor;

public class JsonConsumerService(Options options) : RabbitMQConsumerService<ZendeskTicket>(new ZendeskTicketJsonConsumer(options));

public class MongoDbConsumerService(Options options) : RabbitMQConsumerService<ZendeskTicket>(new ZendeskTicketMongoDbConsumer(options));

public class QdrantConsumerService(Options options) : RabbitMQConsumerService<ZendeskTicket>(new ZendeskTicketQdrantConsumer(options));

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