using NexAI.Zendesk.Queries;
using Spectre.Console;

namespace NexAI.Console.Features;

public class SummarizeZendeskTicketFeature(
    GetZendeskTicketByExternalIdQuery getZendeskTicketByExternalIdQuery,
    StreamZendeskTicketSummaryQuery streamZendeskTicketSummaryQuery)
{
    public async Task Run(CancellationToken cancellationToken)
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[Aquamarine1]Welcome to Ticket Summarizer! Enter Zendesk ticket id. Type [bold]STOP[/] to exit.[/]");
            var userMessage = AnsiConsole.Prompt(new TextPrompt<string>("> "));
            if (userMessage.ToUpper() == "STOP")
            {
                return;
            }
            try
            {
                AnsiConsole.Write(new Rule("[bold]Fetching data.[/]"));
                await SummarizeZendeskTicket(userMessage, cancellationToken);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message.EscapeMarkup()}[/]");
            }
            AnsiConsole.Write(new Rule());
        }
    }

    private async Task SummarizeZendeskTicket(string userMessage, CancellationToken cancellationToken)
    {
        var zendeskTicket = await getZendeskTicketByExternalIdQuery.Handle(userMessage, cancellationToken);
        if (zendeskTicket == null)
        {
            AnsiConsole.MarkupLine("[red]No Zendesk ticket found with that id.[/]");
            return;
        }
        AnsiConsole.MarkupLine("[bold Aquamarine1]Ticket Summary:[/]");
        await foreach (var chunk in streamZendeskTicketSummaryQuery.Handle(zendeskTicket, cancellationToken))
            AnsiConsole.Markup(chunk.EscapeMarkup());
    }
}