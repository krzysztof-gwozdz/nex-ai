using NexAI.Config;
using NexAI.RabbitMQ;
using NexAI.Zendesk;

namespace NexAI.DataProcessor.Zendesk;

public class ZendeskTicketJsonConsumer(Options options) : RabbitMQConsumer<ZendeskTicket>(options, "json")
{
    protected override async Task HandleMessage(ZendeskTicket zendeskTicket)
    {
        var zendeskTicketJsonExporter = new ZendeskTicketJsonExporter(options);
        await zendeskTicketJsonExporter.Export(zendeskTicket);
    }
}   