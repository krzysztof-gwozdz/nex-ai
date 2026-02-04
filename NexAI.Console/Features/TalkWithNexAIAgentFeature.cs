using NexAI.Agents;
using NexAI.LLMs.Common;
using Spectre.Console;

namespace NexAI.Console.Features;

public class TalkWithNexAIAgentFeature(NexAIAgent nexAIAgent)
{
    public async Task Run(CancellationToken cancellationToken)
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[Aquamarine1]Welcome to Nex AI! Type your message below. Type [bold]RESET[/] to reset the conversation or [bold]STOP[/] to exit.[/]");
            nexAIAgent.StartNewChat(ConversationId.New());
            while (true)
            {
                var userMessage = AnsiConsole.Prompt(new TextPrompt<string>(">"));
                if (userMessage == "RESET")
                    break;
                if (userMessage == "STOP")
                    return;
                var response = await nexAIAgent.Ask(nexAIAgent.ConversationId, userMessage, cancellationToken);
                AnsiConsole.MarkupLine($"[Aquamarine1]{response.EscapeMarkup()}[/]");
            }
            
            AnsiConsole.Write(new Rule());
        }
    }
}