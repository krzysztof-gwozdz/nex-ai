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
            AnsiConsole.MarkupLine($"[green]{ticket.Subject.EscapeMarkup()}[/]");
            AnsiConsole.MarkupLine($"[green]Comments:[/]");
            var comments = await zendeskApiClient.GetTicketComments(ticket.Id!.Value, 5);
            foreach (var comment in comments)
            {
                AnsiConsole.MarkupLine($"[green]{comment.PlainBody.EscapeMarkup()}[/]");
            }
            AnsiConsole.MarkupLine("");
        }

        var employeesCount = await zendeskApiClient.GetEmployeesCount();
        AnsiConsole.MarkupLine($"[yellow]Zendesk employees count: {employeesCount}[/]");
        
        var employees = await zendeskApiClient.GetEmployees(10);
        AnsiConsole.MarkupLine("First 10 employees names:");
        foreach (var employee in employees)
        {
            AnsiConsole.MarkupLine($"[green]{employee.Name.EscapeMarkup()} ({employee.Role.EscapeMarkup()})[/]");
        }   

        return [];
    }
}