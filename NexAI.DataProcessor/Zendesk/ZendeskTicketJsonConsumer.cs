using Microsoft.Extensions.Logging;
using NexAI.Config;
using NexAI.RabbitMQ;
using NexAI.Zendesk;

namespace NexAI.DataProcessor.Zendesk;

public class ZendeskTicketJsonConsumer(ILogger logger, Options options) : RabbitMQConsumer<ZendeskTicket>(logger, options, "json")
{
    protected override async Task HandleMessage(ZendeskTicket zendeskTicket)
    {
        var zendeskTicketJsonExporter = new ZendeskTicketJsonExporter(options);
        await zendeskTicketJsonExporter.Export(zendeskTicket);
    }
}