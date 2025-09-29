using System.Runtime.CompilerServices;
using System.Text.Json;
using NexAI.Config;
using NexAI.LLMs.Common;
using OpenAI.Chat;

namespace NexAI.LLMs.OpenAI;

public class OpenAIChat(Options options) : Chat
{
    private readonly ChatClient _chatClient = new(
        options.Get<OpenAIOptions>().ChatModel,
        options.Get<OpenAIOptions>().ApiKey
    );

    public override async Task<string> Ask(string systemMessage, string message, CancellationToken cancellationToken)
    {
        List<ChatMessage> messages =
        [
            ChatMessage.CreateSystemMessage(systemMessage),
            ChatMessage.CreateUserMessage(message)
        ];
        var result = await _chatClient.CompleteChatAsync(messages, cancellationToken: cancellationToken);
        var response = result?.Value?.Content[0]?.Text ?? string.Empty;
        return response;
    }

    public override async Task<TResponse> Ask<TResponse>(string systemMessage, string message, CancellationToken cancellationToken)
    {
        List<ChatMessage> messages =
        [
            ChatMessage.CreateSystemMessage(systemMessage),
            ChatMessage.CreateUserMessage(message)
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
        List<ChatMessage> messages =
        [
            ChatMessage.CreateSystemMessage(systemMessage),
            ChatMessage.CreateUserMessage(message)
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
}