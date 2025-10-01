using System.Text;
using NexAI.Zendesk.Queries;
using Spectre.Console;

namespace NexAI.Console.Features;

public class SearchForZendeskTicketsByPhraseFeature(
    GetZendeskTicketByExternalIdQuery getZendeskTicketByExternalIdQuery,
    FindSimilarZendeskTicketsByPhraseQuery findSimilarZendeskTicketsByPhraseQuery,
    FindZendeskTicketsThatContainPhraseQuery findZendeskTicketsThatContainPhraseQuery
)
{
    public async Task Run(int limit, CancellationToken cancellationToken)
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[Aquamarine1]Welcome to Tickets Search! Enter search phrase. Type [bold]STOP[/] to exit.[/]");
            var zendeskTicket = await getZendeskTicketByExternalIdQuery.Handle("841252", cancellationToken);
            var textBuilder = new StringBuilder();
            textBuilder.AppendLine(zendeskTicket.Title);
            textBuilder.AppendLine(zendeskTicket.Description);
            foreach (var message in zendeskTicket.Messages.OrderBy(message => message.CreatedAt))
            {
                textBuilder.AppendLine(message.Content);
            }
            var userMessage = textBuilder.ToString();

            //AnsiConsole.Prompt(new TextPrompt<string>("> "));
            if (userMessage.ToUpper() == "STOP")
                return;
            try
            {
                AnsiConsole.Write(new Rule($"[bold]Searching for up to {limit} tickets for phrase: {userMessage.EscapeMarkup()}[/]"));

                var similarSearchResults = await findSimilarZendeskTicketsByPhraseQuery.Handle(userMessage, limit, cancellationToken);
                var phraseSearchResults = await findZendeskTicketsThatContainPhraseQuery.Handle(userMessage, limit, cancellationToken);

                var searchResults = similarSearchResults.Concat(phraseSearchResults).ToArray();

                var table = new Table().AddColumn("Method").AddColumn("External Id").AddColumn("Title").AddColumn("Score").AddColumn("Info");
                foreach (var searchResult in searchResults)
                {
                    var color = similarSearchResults.Any(x => x.ZendeskTicket.Id == searchResult.ZendeskTicket.Id) && phraseSearchResults.Any(x => x.ZendeskTicket.Id == searchResult.ZendeskTicket.Id) ? "green" : similarSearchResults.Any(x => x.ZendeskTicket.Id == searchResult.ZendeskTicket.Id) ? "yellow" : "blue";
                    table.AddRow(
                        $"[{color}]{searchResult.Method}[/]",
                        $"[{color}]{searchResult.ZendeskTicket.ExternalId}[/]",
                        $"[{color}]{searchResult.ZendeskTicket.Title.EscapeMarkup()}[/]",
                        $"[{color}]{searchResult.Score:P1}[/]",
                        $"[{color}]{searchResult.Info}[/]"
                    );
                }
                AnsiConsole.Write(table);
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
        var searchResults = await findSimilarZendeskTicketsByPhraseQuery.Handle(userMessage, limit, cancellationToken);
        AnsiConsole.MarkupLine("[bold Aquamarine1]Similar tickets (embedding):[/]");
        if (searchResults.Length == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No similar tickets found.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[bold]Found {searchResults.Length} tickets:[/]");
            var table = new Table().AddColumn("External Id").AddColumn("Title").AddColumn("Score").AddColumn("Info");
            foreach (var searchResult in searchResults)
            {
                table.AddRow(
                    searchResult.ZendeskTicket.ExternalId,
                    searchResult.ZendeskTicket.Title.EscapeMarkup(),
                    searchResult.Score.ToString("P1"),
                    searchResult.Info
                );
            }
            AnsiConsole.Write(table);
        }
    }

    private async Task GetZendeskTicketsByPhrase(string userMessage, int limit, CancellationToken cancellationToken)
    {
        var searchResults = await findZendeskTicketsThatContainPhraseQuery.Handle(userMessage, limit, cancellationToken);
        AnsiConsole.MarkupLine("[bold Aquamarine1]Tickets that contain phrase (full text search):[/]");
        if (searchResults.Length == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No similar tickets found.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[bold]Found {searchResults.Length} tickets:[/]");
            var table = new Table().AddColumn("External Id").AddColumn("Title").AddColumn("Score").AddColumn("Info");
            foreach (var searchResult in searchResults)
            {
                table.AddRow(
                    searchResult.ZendeskTicket.ExternalId,
                    searchResult.ZendeskTicket.Title.EscapeMarkup(),
                    searchResult.Score.ToString("P1"),
                    searchResult.Info
                );
            }
            AnsiConsole.Write(table);
        }
    }
}