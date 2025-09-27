using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using NexAI.AzureDevOps;
using NexAI.Config;
using NexAI.Console.Features;
using NexAI.Console.Plugins;
using NexAI.LLMs;
using NexAI.LLMs.Common;
using NexAI.LLMs.Ollama;
using NexAI.LLMs.OpenAI;
using NexAI.MongoDb;
using NexAI.Qdrant;
using NexAI.RabbitMQ;
using NexAI.Zendesk;
using Spectre.Console;

namespace NexAI.Console;

public class NexAIAgent
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatCompletionService;
    private readonly OpenAIPromptExecutionSettings _openAIPromptExecutionSettings;

    public NexAIAgent(Options options)
    {
        var mode = options.Get<LLMsOptions>().Mode;
        var builder = GetKernelBuilder(options, mode);
        _kernel = builder.Build();
        _kernel.Plugins.AddFromType<ZendeskTicketsPlugin>("ZendeskTickets", _kernel.Services);
        _kernel.Plugins.AddFromType<AzureDevOpsIssuesPlugin>("AzureDevOpsIssues", _kernel.Services);
        _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>(mode);
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

    private static IKernelBuilder GetKernelBuilder(Options options, string mode)
    {
        var builder = Kernel.CreateBuilder();
        switch (mode)
        {
            case LLM.OpenAI:
                var openAIOptions = options.Get<OpenAIOptions>();
                builder.AddOpenAIChatCompletion(openAIOptions.ChatModel, openAIOptions.ApiKey, serviceId: mode);
                AnsiConsole.MarkupLine($"[red]Using {mode} model: {openAIOptions.ChatModel}[/]");
                break;
            case LLM.Ollama:
                var ollamaOptions = options.Get<OllamaOptions>();
                builder.AddOllamaChatCompletion(ollamaOptions.ChatModel, ollamaOptions.BaseAddress, serviceId: mode);
                AnsiConsole.MarkupLine($"[green]Using {mode} model: {ollamaOptions.ChatModel}[/]");
                break;
        }
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton(options);
        builder.Services.AddAzureDevOps();
        builder.Services.AddZendesk();
        builder.Services.AddMongoDb();
        builder.Services.AddQdrant();
        builder.Services.AddRabbitMQ();
        builder.Services.AddLLM(options);
        builder.Services.AddSingleton<SummarizeZendeskTicketFeature>();
        builder.Services.AddSingleton<SearchForZendeskTicketsByPhraseFeature>();
        builder.Services.AddSingleton<SearchForAzureWorkItemsByPhraseFeature>();
        builder.Services.AddSingleton<SearchForInfoAboutTicketFeature>();
        return builder;
    }

    private async Task<ChatMessageContent> GetAIResponse(ChatHistory chatHistory) =>
        await _chatCompletionService.GetChatMessageContentAsync(
            chatHistory,
            executionSettings: _openAIPromptExecutionSettings,
            kernel: _kernel);
}