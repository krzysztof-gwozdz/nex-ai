using NexAI.Zendesk.Queries;
using Spectre.Console;

namespace NexAI.Console.Features;

public class SearchForZendeskTicketsByPhraseFeature(
    FindSimilarZendeskTicketsByPhraseQuery findSimilarZendeskTicketsByPhraseQuery,
    FindZendeskTicketsThatContainPhraseQuery findZendeskTicketsThatContainPhraseQuery
)
{
    public async Task Run(int limit, CancellationToken cancellationToken)
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
                await GetSimilarZendeskTicketsByPhrase(userMessage, limit, cancellationToken);
                await GetZendeskTicketsByPhrase(userMessage, limit, cancellationToken);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message.EscapeMarkup()}[/]");
            }

            AnsiConsole.Write(new Rule());
        }
    }

    private async Task GetSimilarZendeskTicketsByPhrase(string userMessage, int limit, CancellationToken cancellationToken)
    {
        var searchResult = await findSimilarZendeskTicketsByPhraseQuery.Handle(userMessage, limit, cancellationToken);
        AnsiConsole.MarkupLine("[bold Aquamarine1]Similar tickets (embedding):[/]");
        if (searchResult.Length == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No similar tickets found.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[bold]Found {searchResult.Length} tickets:[/]");
            var table = new Table().AddColumn("External Id").AddColumn("Title").AddColumn("Description").AddColumn("Score");
            var ticketsWithSimilarities = searchResult
                .Select(result => new
                {
                    result.ZendeskTicket.ExternalId,
                    result.ZendeskTicket.Title,
                    result.ZendeskTicket.Description,
                    result.Score
                })
                .OrderByDescending(ticket => ticket.Score)
                .ToList();
            foreach (var ticket in ticketsWithSimilarities)
            {
                table.AddRow(
                    ticket.ExternalId,
                    ticket.Title.EscapeMarkup(),
                    ticket.Description[..50].EscapeMarkup(),
                    ticket.Score.ToString("P1")
                );
            }

            AnsiConsole.Write(table);
        }
    }

    private async Task GetZendeskTicketsByPhrase(string userMessage, int limit, CancellationToken cancellationToken)
    {
        var searchResult = await findZendeskTicketsThatContainPhraseQuery.Handle(userMessage, limit, cancellationToken);
        AnsiConsole.MarkupLine("[bold Aquamarine1]Tickets that contain phrase (full text search):[/]");
        if (searchResult.Length == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No similar tickets found.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[bold]Found {searchResult.Length} tickets:[/]");
            var table = new Table().AddColumn("External Id").AddColumn("Title").AddColumn("Description").AddColumn("Score");
            foreach (var result in searchResult)
            {
                table.AddRow(
                    result.ZendeskTicket.ExternalId.EscapeMarkup(),
                    result.ZendeskTicket.Title.EscapeMarkup(),
                    result.ZendeskTicket.Description[..50].EscapeMarkup(),
                    result.Score.ToString("0.00")
                );
            }

            AnsiConsole.Write(table);
        }
    }
}