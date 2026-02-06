using System.Text.Json;
using System.Text.Json.Schema;

namespace NexAI.LLMs.Common;

public abstract class Chat
{
    public abstract string Provider { get; }
    public abstract string Model { get; }
    
    public abstract Task<string> Ask(ConversationId conversationId, string systemMessage, string message, CancellationToken cancellationToken);

    public abstract Task<TResponse> Ask<TResponse>(ConversationId conversationId, string systemMessage, string message, CancellationToken cancellationToken) where TResponse : class;

    public abstract IAsyncEnumerable<string> AskStream(ConversationId conversationId, string systemMessage, string message, CancellationToken cancellationToken);

    public abstract Task<string> GetNextResponse(ConversationId conversationId, ChatMessage[] messages, CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<string> StreamNextResponse(ConversationId conversationId, ChatMessage[] messages, CancellationToken cancellationToken);

    protected ChatMessage[] ToMessages(string systemMessage, string message) =>
        [new("system", systemMessage), new("user", message)];

    protected static string GetSchema<TResponse>()
    {
        var schemaNode = JsonSerializerOptions.Default.GetJsonSchemaAsNode(typeof(TResponse));
        schemaNode["type"] = "object";
        schemaNode["additionalProperties"] = false;
        return schemaNode.ToJsonString();
    }
}