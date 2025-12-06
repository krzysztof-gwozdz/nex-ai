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
    private readonly ChatHistory _chatHistory = new();

    public NexAIAgent(Options options)
    {
        var mode = options.Get<LLMsOptions>().Mode;
        var builder = GetKernelBuilder(options, mode);
        _kernel = builder.Build();
        _kernel.Plugins.AddFromType<ZendeskTicketsPlugin>("ZendeskTickets", _kernel.Services);
        // _kernel.Plugins.AddFromType<AzureDevOpsIssuesPlugin>("AzureDevOpsIssues", _kernel.Services);
        _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>(mode);
        _openAIPromptExecutionSettings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        };
    }

    public void StartNewConversation(Message[]? messages = null)
    {
        _chatHistory.Clear();
        if (messages is not null)
        {
            foreach (var message in messages)
            {
                _chatHistory.AddMessage(new(message.Role), message.Content);
            }
        }
    }

    public async Task<string> Ask(string userMessage, CancellationToken cancellationToken)
    {
        _chatHistory.AddUserMessage(userMessage);
        return await GetResponse(cancellationToken);
    }
    
    public async Task<string> GetResponse(CancellationToken cancellationToken)
    {
        var response = await _chatCompletionService.GetChatMessageContentAsync(
            _chatHistory,
            executionSettings: _openAIPromptExecutionSettings,
            kernel: _kernel, cancellationToken: cancellationToken);
        var assistantResponse = response.Content ?? string.Empty;
        _chatHistory.AddMessage(response.Role, assistantResponse);
        return assistantResponse;
    }

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
    
    public record Message(string Role, string Content);
}