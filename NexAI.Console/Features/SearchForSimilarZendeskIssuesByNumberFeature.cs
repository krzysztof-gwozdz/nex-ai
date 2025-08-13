using NexAI.Config;
using NexAI.Zendesk;
using NexAI.Zendesk.Queries;
using Spectre.Console;

namespace NexAI.Console.Features;

public class SearchForSimilarZendeskIssuesByNumberFeature(Options options)
{
    public async Task Run(ulong limit)
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[Aquamarine1]Welcome to Similar Issues Search! Enter an issue number to find similar issues. Type [bold]STOP[/] to exit.[/]");
            var userMessage = AnsiConsole.Prompt(new TextPrompt<string>("Issue number > "));
            if (userMessage.ToUpper() == "STOP")
                return;

            if (string.IsNullOrWhiteSpace(userMessage))
            {
                AnsiConsole.MarkupLine("[Aquamarine1]Please enter a valid issue number.[/]");
                continue;
            }

            try
            {
                var targetIssue = await new GetZendeskIssueByNumberQuery(options).Handle(userMessage);
                if (targetIssue is null)
                {
                    AnsiConsole.MarkupLine($"[red]Issue with number '{userMessage.EscapeMarkup()}' not found.[/]");
                    continue;
                }

                var similarIssues = await new FindSimilarZendeskIssuesByNumberQuery(options).Handle(userMessage, limit);
                AnsiConsole.MarkupLine($"[green]Found issue: {targetIssue.Title.EscapeMarkup()}[/]");
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