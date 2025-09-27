using NexAI.AzureDevOps;
using NexAI.AzureDevOps.Queries;
using NexAI.Zendesk;
using NexAI.Zendesk.Queries;
using Spectre.Console;

namespace NexAI.Console.Features;

public class SearchForInfoAboutTicketFeature(
    GetZendeskTicketByExternalIdQuery getZendeskTicketByExternalIdQuery,
    GetAzureDevopsWorkItemsRelatedToZendeskTicketQuery getAzureDevopsWorkItemsRelatedToZendeskTicketQuery)
{
    public async Task Run()
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[Aquamarine1]Welcome to Ticket Info Fetcher! Enter Zendesk ticket id. Type [bold]STOP[/] to exit.[/]");
            var userMessage = AnsiConsole.Prompt(new TextPrompt<string>("> "));
            if (userMessage.ToUpper() == "STOP")
                return;
            try
            {
                AnsiConsole.Write(new Rule("[bold]Fetching data.[/]"));
                await FetchZendeskTicketInfo(userMessage);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message.EscapeMarkup()}[/]");
            }

            AnsiConsole.Write(new Rule());
        }
    }

    private async Task FetchZendeskTicketInfo(string userMessage)
    {
        var zendeskTicket = await getZendeskTicketByExternalIdQuery.Handle(userMessage);
        if (zendeskTicket == null)
        {
            AnsiConsole.MarkupLine("[red]No Zendesk ticket found with that id.[/]");
            return;
        }
        DisplayZendeskTicket(zendeskTicket);
        var azureDevOpsWorkItem = await getAzureDevopsWorkItemsRelatedToZendeskTicketQuery.Handle(zendeskTicket.ExternalId, 10);
        DisplayAzureDevOpsWorkItems(azureDevOpsWorkItem);
    }

    private static void DisplayZendeskTicket(ZendeskTicket zendeskTicket)
    {
        AnsiConsole.MarkupLine("[bold Aquamarine1]Found Zendesk ticket:[/]");
        AnsiConsole.MarkupLine($"[bold]Id:[/] {zendeskTicket.ExternalId}");
        AnsiConsole.MarkupLine($"[bold]Title:[/] {zendeskTicket.Title.EscapeMarkup()}");
        AnsiConsole.MarkupLine($"[bold]Description:[/] {zendeskTicket.Description.EscapeMarkup()}");
        AnsiConsole.MarkupLine("[bold]Messages:[/]");
        if (zendeskTicket.Messages.Length == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No messages found for this ticket.[/]");
            return;
        }
        foreach (var message in zendeskTicket.Messages)
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