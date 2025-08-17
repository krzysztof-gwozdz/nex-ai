using NexAI.Config;
using NexAI.Zendesk;
using NexAI.Zendesk.Api;
using Spectre.Console;

namespace NexAI.DataImporter.Zendesk;

internal class ZendeskTicketImporter(Options options)
{
    public async Task<ZendeskTicket[]> Import()
    {
        AnsiConsole.MarkupLine("[yellow]Importing sample Zendesk tickets from JSON...[/]");
        var zendeskApiClient = new ZendeskApiClient(options);

        var employees = await zendeskApiClient.GetEmployees();
        var zendeskTickets = new List<ZendeskTicket>();
        var tickets = await zendeskApiClient.GetTickets(5); // todo remove limit when finish testing
        foreach (var ticket in tickets)
        {
            var comments = await zendeskApiClient.GetTicketComments(ticket.Id!.Value);
            zendeskTickets.Add(new(
                Guid.CreateVersion7(),
                ticket.Id.Value.ToString(),
                ticket.Subject ?? "<MISSING TITLE>",
                ticket.Description ?? "<MISSING DESCRIPTION>",
                comments.Select(comment => new ZendeskTicket.ZendeskTicketMessage(
                        comment.PlainBody ?? "<MISSING BODY>",
                        employees.FirstOrDefault(e => e.Id == comment.AuthorId)?.Name ?? "Unknown Author",
                        DateTime.Parse(comment.CreatedAt ?? "<MISSING CREATED AT>")
                    )
                ).ToArray()
            ));
        }

        AnsiConsole.MarkupLine($"[green]Successfully imported {zendeskTickets.Count} Zendesk tickets.[/]");
        foreach (var ticket in zendeskTickets)
        {
            AnsiConsole.MarkupLine($"{ticket.Id} {ticket.Number} {ticket.Title.EscapeMarkup()}");
            AnsiConsole.MarkupLine($"{ticket.Description.EscapeMarkup()}");
            foreach (var message in ticket.Messages)
            {
                AnsiConsole.MarkupLine($"{message.Author.EscapeMarkup()} {message.CreatedAt} {message.Content.EscapeMarkup()}");
            }
        }

        return zendeskTickets.ToArray();
    }
}