using NexAI.Config;
using NexAI.Zendesk.Queries;
using Spectre.Console;

namespace NexAI.Console.Features;

public class SearchForZendeskTicketsByPhraseFeature(Options options)
{
    public async Task Run(int limit)
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[Aquamarine1]Welcome to Tickets Search! Enter search phrase. Type [bold]STOP[/] to exit.[/]");
            var userMessage = AnsiConsole.Prompt(new TextPrompt<string>("> "));
            if (userMessage.ToUpper() == "STOP")
                return;
            try
            {
                AnsiConsole.Write(new Rule($"[bold]Searching for up to {limit} tickets for phrase: {userMessage.EscapeMarkup()}[/]"));
                await GetSimilarZendeskTicketsByPhrase(userMessage, limit);
                await GetZendeskTicketsByPhrase(userMessage, limit);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message.EscapeMarkup()}[/]");
            }

            AnsiConsole.Write(new Rule());
        }
    }

    private async Task GetSimilarZendeskTicketsByPhrase(string userMessage, int limit)
    {
        var similarTickets = await new FindSimilarZendeskTicketsByPhraseQuery(options).Handle(userMessage, limit);
        var zendeskTickets = await new GetZendeskTicketsByNumbersQuery(options).Handle(similarTickets.Select(ticket => ticket.Number).ToArray());
        AnsiConsole.MarkupLine("[bold Aquamarine1]Similar tickets (embedding):[/]");
        if (zendeskTickets.Length == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No similar tickets found.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[bold]Found {zendeskTickets.Length} tickets:[/]");
            var table = new Table().AddColumn("Number").AddColumn("Title").AddColumn("Description").AddColumn("Similarity");
            var ticketsWithSimilarities = zendeskTickets
                .Select(ticket => new
                {
                    ticket.Number,
                    ticket.Title,
                    ticket.Description,
                    similarTickets.FirstOrDefault(similar => similar.Number == ticket.Number)?.Similarity
                })
                .OrderByDescending(ticket => ticket.Similarity)
                .ToList();
            foreach (var ticket in ticketsWithSimilarities)
            {
                table.AddRow(ticket.Number, ticket.Title.EscapeMarkup(), ticket.Description[..50].EscapeMarkup(), ticket.Similarity?.ToString("P1") ?? "N/A");
            }

            AnsiConsole.Write(table);
        }
    }

    private async Task GetZendeskTicketsByPhrase(string userMessage, int limit)
    {
        var zendeskTickets = await new FindZendeskTicketsThatContainPhraseQuery(options).Handle(userMessage, limit);
        AnsiConsole.MarkupLine("[bold Aquamarine1]Tickets that contain phrase (full text search):[/]");
        if (zendeskTickets.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No similar tickets found.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[bold]Found {zendeskTickets.Count} tickets:[/]");
            var table = new Table().AddColumn("Number").AddColumn("Title").AddColumn("Description").AddColumn("Score");
            foreach (var (ticket, score) in zendeskTickets)
            {
                table.AddRow(ticket.Number.EscapeMarkup(), ticket.Title.EscapeMarkup(), ticket.Description[..50].EscapeMarkup(), score.ToString("0.00"));
            }

            AnsiConsole.Write(table);
        }
    }
}