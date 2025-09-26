using NexAI.Config;
using NexAI.Zendesk.Queries;
using Spectre.Console;

namespace NexAI.Console.Features;

public class SummarizeZendeskTicketFeature(Options options)
{
    public async Task Run()
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[Aquamarine1]Welcome to Ticket Summarizer! Enter Zendesk ticket id. Type [bold]STOP[/] to exit.[/]");
            var userMessage = AnsiConsole.Prompt(new TextPrompt<string>("> "));
            if (userMessage.ToUpper() == "STOP")
                return;
            try
            {
                AnsiConsole.Write(new Rule("[bold]Fetching data.[/]"));
                await SummarizeZendeskTicket(userMessage);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message.EscapeMarkup()}[/]");
            }
            AnsiConsole.Write(new Rule());
        }
    }

    private async Task SummarizeZendeskTicket(string userMessage)
    {
        var zendeskTicket = await new GetZendeskTicketByExternalIdQuery(options).Handle(userMessage);
        if (zendeskTicket == null)
        {
            AnsiConsole.MarkupLine("[red]No Zendesk ticket found with that id.[/]");
            return;
        }
        AnsiConsole.MarkupLine("[bold Aquamarine1]Ticket Summary:[/]");
        await foreach (var chunk in new StreamZendeskTicketSummaryQuery(options).Handle(zendeskTicket))
            AnsiConsole.Markup(chunk.EscapeMarkup());
    }
}