using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using NexAI.Config;
using NexAI.LLMs;
using Spectre.Console;

namespace NexAI.Console;

public class NexAIAgent
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatCompletionService;
    private readonly OpenAIPromptExecutionSettings _openAIPromptExecutionSettings;

    public NexAIAgent(Options options)
    {
        var openAIOptions = options.Get<OpenAIOptions>();
        var ollamaOptions = options.Get<OllamaOptions>();
        var builder = Kernel.CreateBuilder()
                .AddOpenAIChatCompletion(openAIOptions.ChatModel, openAIOptions.ApiKey, serviceId: "openAI")
                .AddOllamaChatCompletion(ollamaOptions.ChatModel, ollamaOptions.BaseAddress, serviceId: "ollama");
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton(options);
        _kernel = builder.Build();
        _kernel.Plugins.AddFromType<ZendeskPlugin>("ZendeskTickets", _kernel.Services);
        _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>("openAI");
        _openAIPromptExecutionSettings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        };
    }

    public async Task StartConversation()
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
                var result = await GetAIResponse(chatHistory);
                var assistantResponse = result.Content ?? string.Empty;
                AnsiConsole.MarkupLine($"[Aquamarine1]{assistantResponse.EscapeMarkup()}[/]");
                chatHistory.AddMessage(result.Role, assistantResponse);
            }

            AnsiConsole.Write(new Rule());
        }
    }

    private async Task<ChatMessageContent> GetAIResponse(ChatHistory chatHistory) =>
        await _chatCompletionService.GetChatMessageContentAsync(
            chatHistory,
            executionSettings: _openAIPromptExecutionSettings,
            kernel: _kernel);
}