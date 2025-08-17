using NexAI.AzureDevOps.Queries;
using NexAI.Config;
using Spectre.Console;

namespace NexAI.Console.Features;

public class SearchForAzureWorkItemsByPhraseFeature(Options options)
{
    public async Task Run(int limit)
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[Aquamarine1]Welcome to Work Item Search! Enter search phrase. Type [bold]STOP[/] to exit.[/]");
            var userMessage = AnsiConsole.Prompt(new TextPrompt<string>("> "));
            if (userMessage.ToUpper() == "STOP")
                return;
            try
            {
                AnsiConsole.Write(new Rule($"[bold]Searching for up to {limit} tickets for phrase: {userMessage.EscapeMarkup()}[/]"));
                await GetAzureDevOpsWorkItemByPhrase(userMessage, limit);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message.EscapeMarkup()}[/]");
            }

            AnsiConsole.Write(new Rule());
        }
    }

    private async Task GetAzureDevOpsWorkItemByPhrase(string phrase, int limit)
    {
        var azureDevOpsWorkItems = await new GetAzureDevopsWorkItemsQuery(options).Handle(phrase, limit);
        AnsiConsole.MarkupLine("[bold Aquamarine1]Work items that contain phrase:[/]");
        if (azureDevOpsWorkItems.Length == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No work items found.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[bold]Found {azureDevOpsWorkItems.Length} work items:[/]");
            var table = new Table().AddColumn("Id").AddColumn("Title").AddColumn("Description").AddColumn("State").AddColumn("AssignedTo");
            foreach (var ticket in azureDevOpsWorkItems)
            {   
                table.AddRow(ticket.Id.EscapeMarkup(), ticket.Title.EscapeMarkup(), ticket.Description.EscapeMarkup(), ticket.State.EscapeMarkup(), ticket.AssignedTo.EscapeMarkup());
            }

            AnsiConsole.Write(table);
        }
    }
}