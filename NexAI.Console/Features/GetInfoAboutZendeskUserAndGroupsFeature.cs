using NexAI.Zendesk.Queries;
using Spectre.Console;

namespace NexAI.Console.Features;

public class GetInfoAboutZendeskUserAndGroupsFeature(GetInfoAboutZendeskHierarchyQuery getInfoAboutZendeskHierarchyQuery)
{
    public async Task Run(CancellationToken cancellationToken)
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[Aquamarine1]Welcome to User and Groups Info Fetcher! Enter Zendesk user id. Type [bold]STOP[/] to exit.[/]");
            var userMessage = AnsiConsole.Prompt(new TextPrompt<string>("> "));
            if (userMessage.ToUpper() == "STOP")
            {
                return;
            }
            try
            {
                AnsiConsole.Write(new Rule("[bold]Fetching data.[/]"));
                var answer = await getInfoAboutZendeskHierarchyQuery.Handle(userMessage);
                AnsiConsole.WriteLine(answer);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message.EscapeMarkup()}[/]");
            }
            AnsiConsole.Write(new Rule());
        }
    }
}