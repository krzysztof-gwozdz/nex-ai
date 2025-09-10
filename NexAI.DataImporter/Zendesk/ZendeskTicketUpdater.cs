using NexAI.Config;
using NexAI.RabbitMQ;
using Spectre.Console;

namespace NexAI.DataImporter.Zendesk;

public class ZendeskTicketUpdater(Options options)
{
    public async Task Update()
    {
        var rabbitMQClient = new RabbitMQClient(options.Get<RabbitMQOptions>());
        var importer = new ZendeskTicketImporter(options);
        var zendeskTickets = await importer.Import();
        foreach (var zendeskTicket in zendeskTickets)
        {
            AnsiConsole.MarkupLine($"[darkgoldenrod]Sending Zendesk ticket {zendeskTicket.Id} to RabbitMQ...[/]");
            await rabbitMQClient.Send(RabbitMQ.ExchangeName, zendeskTicket);
        }
    }
}