using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using NexAI.Agents.Plugins;
using NexAI.AzureDevOps;
using NexAI.Config;
using NexAI.LLMs;
using NexAI.LLMs.Common;
using NexAI.LLMs.Ollama;
using NexAI.LLMs.OpenAI;
using NexAI.MongoDb;
using NexAI.Neo4j;
using NexAI.Qdrant;
using NexAI.Zendesk;

namespace NexAI.Agents;

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

    public async Task<ChatMessageContent> Ask(ChatHistory chatHistory, CancellationToken cancellationToken) =>
        await _chatCompletionService.GetChatMessageContentAsync(
            chatHistory,
            executionSettings: _openAIPromptExecutionSettings,
            kernel: _kernel, cancellationToken: cancellationToken);

    private static IKernelBuilder GetKernelBuilder(Options options, string mode)
    {
        var builder = Kernel.CreateBuilder();
        switch (mode)
        {
            case LLM.OpenAI:
                var openAIOptions = options.Get<OpenAIOptions>();
                builder.AddOpenAIChatCompletion(openAIOptions.ChatModel, openAIOptions.ApiKey, serviceId: mode);
                break;
            case LLM.Ollama:
                var ollamaOptions = options.Get<OllamaOptions>();
                builder.AddOllamaChatCompletion(ollamaOptions.ChatModel, ollamaOptions.BaseAddress, serviceId: mode);
                break;
        }
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton(options);
        builder.Services.AddAzureDevOps();
        builder.Services.AddZendesk();
        builder.Services.AddMongoDb();
        builder.Services.AddNeo4j();
        builder.Services.AddQdrant();
        builder.Services.AddLLM(options);
        return builder;
    }
}