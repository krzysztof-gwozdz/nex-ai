using System.Text.Json;
using NexAI.Config;
using OllamaSharp;

namespace NexAI.LLMs.Ollama;

public class OllamaChat(Options options) : NexAI.LLMs.Common.Chat
{
    private readonly OllamaApiClient _apiClient = new(
        options.Get<OllamaOptions>().BaseAddress,
        options.Get<OllamaOptions>().ChatModel
    );

    public override async Task<string> Ask(string systemMessage, string message, CancellationToken cancellationToken)
    {
        var chat = new Chat(_apiClient, systemMessage);
        var response = string.Empty;
        await foreach (var chunk in chat.SendAsync(message, cancellationToken))
            response += chunk;
        return response;
    }

    public override async Task<TResponse> Ask<TResponse>(string systemMessage, string message, CancellationToken cancellationToken)
    {
        var schema = GetSchema<TResponse>();
        systemMessage += $"\nRespond in JSON format only that adheres to the following schema:\n{schema}";
        var response = await Ask(systemMessage, message, cancellationToken);
        return JsonSerializer.Deserialize<TResponse>(response) ?? throw new JsonException($"Failed to deserialize response to {typeof(TResponse).Name}");
    }

    public override IAsyncEnumerable<string> AskStream(string systemMessage, string message, CancellationToken cancellationToken)
    {
        var chat = new Chat(_apiClient, systemMessage);
        return chat.SendAsync(message, cancellationToken);
    }
}