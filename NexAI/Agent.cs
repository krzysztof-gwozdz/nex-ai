using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using NexAI.Config;
using Spectre.Console;

namespace NexAI;

public class Agent
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatCompletionService;
    private readonly OpenAIPromptExecutionSettings _openAIPromptExecutionSettings;

    public Agent(Options options)
    {
        var openAIOptions = options.Get<OpenAIOptions>();
        var builder = Kernel.CreateBuilder().AddOpenAIChatCompletion(openAIOptions.Model, openAIOptions.ApiKey);
        _kernel = builder.Build();
        _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        _openAIPromptExecutionSettings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };
    }

    public async Task StartConversation()
    {
        while (true)
        {
            var chatHistory = new ChatHistory();
            while (true)
            {
                var userMessage = AnsiConsole.Prompt(new TextPrompt<string>(">"));
                if (userMessage == "RESET")
                {
                    break;
                }

                if (userMessage == "STOP")
                {
                    return;
                }

                chatHistory.AddUserMessage(userMessage);
                var result = await _chatCompletionService.GetChatMessageContentAsync(
                    chatHistory,
                    executionSettings: _openAIPromptExecutionSettings,
                    kernel: _kernel);
                var assistantResponse = result.Content ?? string.Empty;
                AnsiConsole.MarkupLine($"[Aquamarine1]{assistantResponse.EscapeMarkup()}[/]");
                chatHistory.AddMessage(result.Role, assistantResponse);
            }
            AnsiConsole.Write(new Rule());
        }
    }
}