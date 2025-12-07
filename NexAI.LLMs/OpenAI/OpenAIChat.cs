using System.Runtime.CompilerServices;
using System.Text.Json;
using NexAI.Config;
using NexAI.LLMs.Common;
using OpenAI.Chat;
using ChatMessage = NexAI.LLMs.Common.ChatMessage;
using OpenAIChatMessage = OpenAI.Chat.ChatMessage;

namespace NexAI.LLMs.OpenAI;

public class OpenAIChat(Options options) : Chat
{
    private readonly ChatClient _chatClient = new(
        options.Get<OpenAIOptions>().ChatModel,
        options.Get<OpenAIOptions>().ApiKey
    );

    public override async Task<string> Ask(string systemMessage, string message, CancellationToken cancellationToken)
    {
        List<OpenAIChatMessage> messages =
        [
            OpenAIChatMessage.CreateSystemMessage(systemMessage),
            OpenAIChatMessage.CreateUserMessage(message)
        ];
        var result = await _chatClient.CompleteChatAsync(messages, cancellationToken: cancellationToken);
        var response = result?.Value?.Content[0]?.Text ?? string.Empty;
        return response;
    }

    public override async Task<TResponse> Ask<TResponse>(string systemMessage, string message, CancellationToken cancellationToken)
    {
        List<OpenAIChatMessage> messages =
        [
            OpenAIChatMessage.CreateSystemMessage(systemMessage),
            OpenAIChatMessage.CreateUserMessage(message)
        ];
        var options = new ChatCompletionOptions
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: typeof(TResponse).Name,
                jsonSchema: BinaryData.FromString(GetSchema<TResponse>()),
                jsonSchemaIsStrict: true)
        };
        var result = await _chatClient.CompleteChatAsync(messages, options, cancellationToken);
        var response = result?.Value?.Content[0]?.Text ?? string.Empty;
        return JsonSerializer.Deserialize<TResponse>(response) ?? throw new JsonException($"Failed to deserialize response to {typeof(TResponse).Name}");
    }

    public override async IAsyncEnumerable<string> AskStream(string systemMessage, string message, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        List<OpenAIChatMessage> messages =
        [
            OpenAIChatMessage.CreateSystemMessage(systemMessage),
            OpenAIChatMessage.CreateUserMessage(message)
        ];
        var result = _chatClient.CompleteChatStreamingAsync(messages, cancellationToken: cancellationToken);
        await foreach (var completionUpdate in result)
        {
            if (completionUpdate.ContentUpdate.Count > 0)
            {
                yield return completionUpdate.ContentUpdate[0].Text;
            }
        }
    }

    public override async Task<string> GetNextResponse(ChatMessage[] messages, CancellationToken cancellationToken)
    {
        var result = await _chatClient.CompleteChatAsync(messages.Select(ToOpenAIChatMessage), cancellationToken: cancellationToken);
        var response = result?.Value?.Content[0]?.Text ?? string.Empty;
        return response;
    }

    public override async IAsyncEnumerable<string> StreamNextResponse(ChatMessage[] messages, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var result = _chatClient.CompleteChatStreamingAsync(messages.Select(ToOpenAIChatMessage), cancellationToken: cancellationToken);
        await foreach (var completionUpdate in result)
        {
            if (completionUpdate.ContentUpdate.Count > 0)
            {
                yield return completionUpdate.ContentUpdate[0].Text;
            }
        }
    }

    private static OpenAIChatMessage ToOpenAIChatMessage(ChatMessage message) =>
        message.Role switch
        {
            "system" => OpenAIChatMessage.CreateSystemMessage(message.Content),
            "user" => OpenAIChatMessage.CreateUserMessage(message.Content),
            "assistant" => OpenAIChatMessage.CreateAssistantMessage(message.Content),
            _ => throw new($"Unknown role: {message.Role}")
        };
}