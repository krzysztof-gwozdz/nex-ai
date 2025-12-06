using Microsoft.SemanticKernel.ChatCompletion;
using NexAI.Agents;
using Spectre.Console;

namespace NexAI.Console.Features;

public class TalkWithNexAIAgentFeature(NexAIAgent nexAIAgent)
{
    public async Task Run(CancellationToken cancellationToken)
    {
        while (true)
        {
            var chatHistory = new ChatHistory();
            AnsiConsole.MarkupLine("[Aquamarine1]Welcome to Nex AI! Type your message below. Type [bold]RESET[/] to reset the conversation or [bold]STOP[/] to exit.[/]");
            while (true)
            {
                var userMessage = AnsiConsole.Prompt(new TextPrompt<string>(">"));
                if (userMessage == "RESET")
                    break;
                if (userMessage == "STOP")
                    return;
                chatHistory.AddUserMessage(userMessage);
                var result = await nexAIAgent.Ask(chatHistory, cancellationToken);
                var assistantResponse = result.Content ?? string.Empty;
                AnsiConsole.MarkupLine($"[Aquamarine1]{assistantResponse.EscapeMarkup()}[/]");
                chatHistory.AddMessage(result.Role, assistantResponse);
            }
            AnsiConsole.Write(new Rule());
        }
    }
}