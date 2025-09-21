using Microsoft.Extensions.Logging;
using NexAI.Config;
using NexAI.RabbitMQ;
using NexAI.Zendesk;

namespace NexAI.DataProcessor.Zendesk;

public class ZendeskTicketQdrantConsumer(ILogger logger, Options options) : RabbitMQConsumer<ZendeskTicket>(logger, options, "qdrant")
{
    protected override async Task HandleMessage(ZendeskTicket zendeskTicket)
    {
        var zendeskTicketQdrantExporter = new ZendeskTicketQdrantExporter(options);
        await zendeskTicketQdrantExporter.Export(zendeskTicket);
    }
}   