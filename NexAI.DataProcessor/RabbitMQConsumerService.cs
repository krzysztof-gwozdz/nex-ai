using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NexAI.Config;
using NexAI.DataProcessor.Zendesk;
using NexAI.RabbitMQ;
using NexAI.Zendesk;
using Spectre.Console;

namespace NexAI.DataProcessor;

public class JsonConsumerService(ILogger<JsonConsumerService> logger, ILoggerFactory loggerFactory, Options options) : RabbitMQConsumerService<ZendeskTicket>(new ZendeskTicketJsonConsumer(loggerFactory.CreateLogger<ZendeskTicketJsonConsumer>(), options));

public class MongoDbConsumerService(ILogger<MongoDbConsumerService> logger, ILoggerFactory loggerFactory, Options options) : RabbitMQConsumerService<ZendeskTicket>(new ZendeskTicketMongoDbConsumer(loggerFactory.CreateLogger<ZendeskTicketMongoDbConsumer>(), options));

public class QdrantConsumerService(ILogger<QdrantConsumerService> logger, ILoggerFactory loggerFactory, Options options) : RabbitMQConsumerService<ZendeskTicket>(new ZendeskTicketQdrantConsumer(loggerFactory.CreateLogger<ZendeskTicketQdrantConsumer>(), options));

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