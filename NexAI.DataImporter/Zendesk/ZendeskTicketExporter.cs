using NexAI.Config;
using NexAI.Zendesk;
using Spectre.Console;

namespace NexAI.DataImporter.Zendesk;

public class ZendeskTicketExporter(Options options)
{
    public async Task Export(ZendeskTicket[] zendeskTickets)
    {
        AnsiConsole.MarkupLine("[yellow]Start exporting Zendesk tickets...[/]");
        await new ZendeskTicketQdrantExporter(options).Export(zendeskTickets);
        await new ZendeskTicketMongoDbExporter(options).Export(zendeskTickets);
        AnsiConsole.MarkupLine("[green]Zendesk tickets exported successfully.[/]");
    }
}