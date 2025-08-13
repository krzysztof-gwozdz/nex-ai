using NexAI.Config;
using NexAI.Zendesk.Queries;
using Spectre.Console;

namespace NexAI.Console.Features;

public class SearchForZendeskIssuesByPhraseFeature(Options options)
{
    public async Task Run(int limit)
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[Aquamarine1]Welcome to Issues Search! Enter search phrase. Type [bold]STOP[/] to exit.[/]");
            var userMessage = AnsiConsole.Prompt(new TextPrompt<string>("> "));
            if (userMessage.ToUpper() == "STOP")
                return;
            try
            {
                AnsiConsole.Write(new Rule($"[bold]Searching for up to {limit} issues for phrase: {userMessage.EscapeMarkup()}[/]"));
                await GetSimilarZendeskIssuesByPhrase(userMessage, limit);
                await GetZendeskIssuesByPhrase(userMessage, limit);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message.EscapeMarkup()}[/]");
            }

            AnsiConsole.Write(new Rule());
        }
    }

    private async Task GetSimilarZendeskIssuesByPhrase(string userMessage, int limit)
    {
        var similarIssues = await new FindSimilarZendeskIssuesByPhraseQuery(options).Handle(userMessage, limit);
        var zendeskIssues = await new GetZendeskIssuesByNumbersQuery(options).Handle(similarIssues.Select(issue => issue.Number).ToArray());
        AnsiConsole.MarkupLine("[bold Aquamarine1]Similar issues (embedding):[/]");
        if (zendeskIssues.Length == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No similar issues found.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[bold]Found {zendeskIssues.Length} issues:[/]");
            var table = new Table().AddColumn("Number").AddColumn("Title").AddColumn("Description").AddColumn("Similarity");
            var issuesWithSimilarities = zendeskIssues
                .Select(issue => new
                {
                    issue.Number,
                    issue.Title,
                    issue.Description,
                    similarIssues.FirstOrDefault(similar => similar.Number == issue.Number)?.Similarity
                })
                .OrderByDescending(issue => issue.Similarity)
                .ToList();
            foreach (var issue in issuesWithSimilarities)
            {
                table.AddRow(issue.Number, issue.Title, issue.Description, issue.Similarity?.ToString("P1") ?? "N/A");
            }

            AnsiConsole.Write(table);
        }
    }

    private async Task GetZendeskIssuesByPhrase(string userMessage, int limit)
    {
        var zendeskIssues = await new FindZendeskIssuesThatContainPhraseQuery(options).Handle(userMessage, limit);
        AnsiConsole.MarkupLine("[bold Aquamarine1]Issues that contain phrase (full text search):[/]");
        if (zendeskIssues.Length == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No similar issues found.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[bold]Found {zendeskIssues.Length} issues:[/]");
            var table = new Table().AddColumn("Number").AddColumn("Title").AddColumn("Description");
            foreach (var issue in zendeskIssues)
            {
                table.AddRow(issue.Number.EscapeMarkup(), issue.Title.EscapeMarkup(), issue.Description.EscapeMarkup());
            }

            AnsiConsole.Write(table);
        }
    }
}