using NexAI.Config;
using NexAI.Zendesk;
using NexAI.Zendesk.Queries;
using Spectre.Console;

namespace NexAI.Console.Features;

public class SearchForSimilarZendeskIssuesByPhraseFeature(Options options)
{
    public async Task Run(ulong limit)
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[Aquamarine1]Welcome to Issues Search! Enter search phrase. Type [bold]STOP[/] to exit.[/]");
            var userMessage = AnsiConsole.Prompt(new TextPrompt<string>("> "));
            if (userMessage.ToUpper() == "STOP")
                return;
            try
            {
                var similarIssues = await new FindSimilarZendeskIssuesByPhraseQuery(options).Handle(userMessage, limit);
                DisplaySimilarIssues(similarIssues);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message.EscapeMarkup()}[/]");
            }

            AnsiConsole.Write(new Rule());
        }
    }

    private static void DisplaySimilarIssues(List<SimilarIssue>? similarIssues)
    {
        if (similarIssues is null || similarIssues.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No similar issues found.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[bold]Found {similarIssues.Count} similar issues:[/]");
            var table = new Table().AddColumn("Number").AddColumn("Similarity Score");
            foreach (var issue in similarIssues)
            {
                table.AddRow(issue.Number, $"{issue.Similarity:P1}");
            }
            AnsiConsole.Write(table);
        }
    }
}