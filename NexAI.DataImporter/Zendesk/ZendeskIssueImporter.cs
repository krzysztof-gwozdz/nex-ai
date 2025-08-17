using NexAI.Config;
using NexAI.Zendesk;
using NexAI.Zendesk.Api;
using Spectre.Console;

namespace NexAI.DataImporter.Zendesk;

internal class ZendeskIssueImporter(Options options)
{
    public async Task<ZendeskIssue[]> Import()
    {
        AnsiConsole.MarkupLine("[yellow]Importing sample Zendesk issues from JSON...[/]");
        var zendeskApiClient = new ZendeskApiClient(options);

        var employees = await zendeskApiClient.GetEmployees();
        var zendeskIssues = new List<ZendeskIssue>();
        var tickets = await zendeskApiClient.GetTickets(5); // todo remove limit when finish testing
        foreach (var ticket in tickets)
        {
            var comments = await zendeskApiClient.GetTicketComments(ticket.Id!.Value);
            zendeskIssues.Add(new(
                Guid.CreateVersion7(),
                ticket.Id.Value.ToString(),
                ticket.Subject ?? "<MISSING TITLE>",
                ticket.Description ?? "<MISSING DESCRIPTION>",
                comments.Select(comment => new ZendeskIssue.ZendeskIssueMessage(
                        comment.PlainBody ?? "<MISSING BODY>",
                        employees.FirstOrDefault(e => e.Id == comment.AuthorId)?.Name ?? "Unknown Author",
                        DateTime.Parse(comment.CreatedAt ?? "<MISSING CREATED AT>")
                    )
                ).ToArray()
            ));
        }

        AnsiConsole.MarkupLine($"[green]Successfully imported {zendeskIssues.Count} Zendesk issues.[/]");
        foreach (var issue in zendeskIssues)
        {
            AnsiConsole.MarkupLine($"{issue.Id} {issue.Number} {issue.Title.EscapeMarkup()}");
            AnsiConsole.MarkupLine($"{issue.Description.EscapeMarkup()}");
            foreach (var message in issue.Messages)
            {
                AnsiConsole.MarkupLine($"{message.Author.EscapeMarkup()} {message.CreatedAt} {message.Content.EscapeMarkup()}");
            }
        }

        return zendeskIssues.ToArray();
    }
}