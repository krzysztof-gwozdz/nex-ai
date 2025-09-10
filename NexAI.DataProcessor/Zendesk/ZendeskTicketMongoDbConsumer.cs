using NexAI.Config;
using NexAI.RabbitMQ;
using NexAI.Zendesk;

namespace NexAI.DataProcessor.Zendesk;

public class ZendeskTicketMongoDbConsumer(Options options) : RabbitMQConsumer<ZendeskTicket>(options, "mongodb")
{
    protected override async Task HandleMessage(ZendeskTicket zendeskTicket)
    {
        var zendeskTicketMongoDbExporter = new ZendeskTicketMongoDbExporter(options);
        await zendeskTicketMongoDbExporter.Export(zendeskTicket);
    }
}   