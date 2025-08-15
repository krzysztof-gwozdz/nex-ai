using NexAI.AzureDevOps;
using NexAI.AzureDevOps.Queries;
using NexAI.Config;
using NexAI.Zendesk;
using NexAI.Zendesk.Queries;
using Spectre.Console;

namespace NexAI.Console.Features;

public class SearchForInfoAboutIssueFeature(Options options)
{
    public async Task Run()
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[Aquamarine1]Welcome to Issue Info Fetcher! Enter Zendesk issue number. Type [bold]STOP[/] to exit.[/]");
            var userMessage = AnsiConsole.Prompt(new TextPrompt<string>("> "));
            if (userMessage.ToUpper() == "STOP")
                return;
            try
            {
                AnsiConsole.Write(new Rule("[bold]Fetching data.[/]"));
                await FetchZendeskIssueInfo(userMessage);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message.EscapeMarkup()}[/]");
            }

            AnsiConsole.Write(new Rule());
        }
    }

    private async Task FetchZendeskIssueInfo(string userMessage)
    {
        var zendeskIssue = await new GetZendeskIssueByNumberQuery(options).Handle(userMessage);
        if (zendeskIssue == null)
        {
            AnsiConsole.MarkupLine("[red]No Zendesk issue found with that number.[/]");
            return;
        }
        DisplayZendeskIssue(zendeskIssue);
        var azureDevOpsWorkItem = await new GetAzureDevopsWorkItemsRelatedToZendeskIssueQuery(options).Handle(zendeskIssue.Number, 10);
        DisplayAzureDevOpsWorkItems(azureDevOpsWorkItem);
    }

    private static void DisplayZendeskIssue(ZendeskIssue zendeskIssue)
    {
        AnsiConsole.MarkupLine("[bold Aquamarine1]Found Zendesk issue:[/]");
        AnsiConsole.MarkupLine($"[bold]Number:[/] {zendeskIssue.Number}");
        AnsiConsole.MarkupLine($"[bold]Title:[/] {zendeskIssue.Title.EscapeMarkup()}");
        AnsiConsole.MarkupLine($"[bold]Description:[/] {zendeskIssue.Description.EscapeMarkup()}");
        AnsiConsole.MarkupLine("[bold]Messages:[/]");
        if (zendeskIssue.Messages.Length == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No messages found for this issue.[/]");
            return;
        }
        foreach (var message in zendeskIssue.Messages)
        {
            AnsiConsole.MarkupLine($"[bold]Author:[/] {message.Author.EscapeMarkup()}");
            AnsiConsole.MarkupLine($"[bold]Content:[/] {message.Content.EscapeMarkup()}");
            AnsiConsole.MarkupLine($"[bold]Created At:[/] {message.CreatedAt}");
            AnsiConsole.WriteLine();
        }
    }

    private static void DisplayAzureDevOpsWorkItems(AzureDevOpsWorkItem[] azureDevOpsWorkItems)
    {
        if (azureDevOpsWorkItems.Length == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No related Azure DevOps work items found.[/]");
            return;
        }
        AnsiConsole.MarkupLine("[bold Aquamarine1]Related Azure DevOps work items:[/]");
        var table = new Table()
            .AddColumn("ID")
            .AddColumn("Title")
            .AddColumn("Description")
            .AddColumn("State")
            .AddColumn("Assigned To");
        foreach (var workItem in azureDevOpsWorkItems)
        {
            table.AddRow(
                workItem.Id.EscapeMarkup(),
                workItem.Title.EscapeMarkup(),
                workItem.Description.EscapeMarkup(),
                workItem.State.EscapeMarkup(),
                workItem.AssignedTo.EscapeMarkup());
        }
        AnsiConsole.Write(table);
    }
}