using NexAI.Config;
using OllamaSharp;
using OllamaSharp.Models.Chat;

namespace NexAI.LLMs.Ollama;

public class OllamaChat(Options options) : NexAI.LLMs.Common.Chat
{
    private readonly OllamaApiClient _apiClient = new(
        options.Get<OllamaOptions>().BaseAddress,
        options.Get<OllamaOptions>().ChatModel
    );

    public override async Task<string> Ask(string systemMessage, string message)
    {
        var chat = new Chat(_apiClient, systemMessage);
        var response = string.Empty;
        await foreach (var chunk in chat.SendAsync(message))
            response += chunk;
        return response;
    }

    public override IAsyncEnumerable<string> AskStream(string systemMessage, string message)
    {
        var chat = new Chat(_apiClient, systemMessage);
        return chat.SendAsync(message);
    }
}