using NexAI.Zendesk;
using NexAI.Zendesk.Api;
using Spectre.Console;
using Options = NexAI.Config.Options;

namespace NexAI.DataImporter.Zendesk;

internal class ZendeskIssueImporter(Options options)
{
    public async Task<ZendeskIssue[]> Import()
    {
        AnsiConsole.MarkupLine("[yellow]Importing sample Zendesk issues from JSON...[/]");
        var zendeskApiClient = new ZendeskApiClient(options);
        
        var ticketCount = await zendeskApiClient.GetTicketCount();
        AnsiConsole.MarkupLine($"[yellow]Zendesk tickets count: {ticketCount}[/]");

        var tickets = await zendeskApiClient.GetTickets(10);
        foreach (var ticket in tickets.Take(10))
        {
            AnsiConsole.MarkupLine($"[green]{ticket.Subject}[/]");
        }

        var agentsCount = await zendeskApiClient.GetAgentsCount();
        AnsiConsole.MarkupLine($"[yellow]Zendesk agents count: {agentsCount}[/]");
        
        var agents = await zendeskApiClient.GetAgents(10);
        AnsiConsole.MarkupLine("First 10 agents names:");
        foreach (var agent in agents.Take(10))
        {
            AnsiConsole.MarkupLine($"[green]{agent.Name}[/]");
        }   

        return [];
    }
}