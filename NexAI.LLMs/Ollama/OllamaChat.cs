using System.Text.Json;
using NexAI.Config;
using NexAI.LLMs.Common;
using OllamaSharp;
using OllamaSharp.Models.Chat;
using Chat = OllamaSharp.Chat;

namespace NexAI.LLMs.Ollama;

public class OllamaChat(Options options) : NexAI.LLMs.Common.Chat
{
    private readonly OllamaApiClient _apiClient = new(
        options.Get<OllamaOptions>().BaseAddress,
        options.Get<OllamaOptions>().ChatModel
    );

    public override async Task<string> Ask(ConversationId conversationId, string systemMessage, string message, CancellationToken cancellationToken)
    {
        var chat = new Chat(_apiClient, systemMessage);
        var response = string.Empty;
        await foreach (var chunk in chat.SendAsync(message, cancellationToken))
            response += chunk;
        return response;
    }

    public override async Task<TResponse> Ask<TResponse>(ConversationId conversationId, string systemMessage, string message, CancellationToken cancellationToken)
    {
        var schema = GetSchema<TResponse>();
        systemMessage += $"\nRespond in JSON format only that adheres to the following schema:\n{schema}";
        var response = await Ask(conversationId, systemMessage, message, cancellationToken);
        return JsonSerializer.Deserialize<TResponse>(response) ?? throw new JsonException($"Failed to deserialize response to {typeof(TResponse).Name}");
    }

    public override IAsyncEnumerable<string> AskStream(ConversationId conversationId, string systemMessage, string message, CancellationToken cancellationToken)
    {
        var chat = new Chat(_apiClient, systemMessage);
        return chat.SendAsync(message, cancellationToken);
    }

    public override async Task<string> GetNextResponse(ConversationId conversationId, ChatMessage[] messages, CancellationToken cancellationToken)
    {
        var allMessages = messages.Select(ToOllamaChatMessage).ToList();
        var lastMessage = allMessages.Last();
        allMessages.Remove(lastMessage);
        var chat = new Chat(_apiClient)
        {
            Messages = allMessages
        };
        var response = string.Empty;
        await foreach (var chunk in chat.SendAsync(lastMessage.Content!, cancellationToken))
            response += chunk;
        return response;
    }

    public override IAsyncEnumerable<string> StreamNextResponse(ConversationId conversationId, ChatMessage[] messages, CancellationToken cancellationToken)
    {
        var allMessages = messages.Select(ToOllamaChatMessage).ToList();
        var lastMessage = allMessages.Last();
        allMessages.Remove(lastMessage);
        var chat = new Chat(_apiClient)
        {
            Messages = allMessages
        };
        return chat.SendAsync(lastMessage.Content!, cancellationToken);
    }

    private static Message ToOllamaChatMessage(ChatMessage message) =>
        message.Role switch
        {
            "system" => new(ChatRole.System, message.Content),
            "user" => new(ChatRole.User, message.Content),
            "assistant" => new(ChatRole.Assistant, message.Content),
            _ => throw new($"Unknown role: {message.Role}")
        };
}