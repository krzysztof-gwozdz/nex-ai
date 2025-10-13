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

                var similarSearchResults = await findSimilarZendeskTicketsByPhraseQuery.Handle(userMessage, limit, cancellationToken);
                var phraseSearchResults = await findZendeskTicketsThatContainPhraseQuery.Handle(userMessage, limit, cancellationToken);

                var searchResults = similarSearchResults.Concat(phraseSearchResults).ToArray();

                var table = new Table().AddColumn("Method").AddColumn("External Id").AddColumn("Title").AddColumn("Score").AddColumn("Info");
                foreach (var searchResult in searchResults)
                {
                    var color = similarSearchResults.Any(result => result.ZendeskTicket.Id == searchResult.ZendeskTicket.Id) && phraseSearchResults.Any(x => x.ZendeskTicket.Id == searchResult.ZendeskTicket.Id)
                        ? "green"
                        : similarSearchResults.Any(result => result.ZendeskTicket.Id == searchResult.ZendeskTicket.Id)
                            ? "yellow"
                            : "blue";
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
}