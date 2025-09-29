using System.Text.Json;
using System.Text.Json.Schema;

namespace NexAI.LLMs.Common;

public abstract class Chat
{
    public abstract Task<string> Ask(string systemMessage, string message, CancellationToken cancellationToken);

    public abstract Task<TResponse> Ask<TResponse>(string systemMessage, string message, CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<string> AskStream(string systemMessage, string message, CancellationToken cancellationToken);

    protected static string GetSchema<TResponse>()
    {
        var schemaNode = JsonSerializerOptions.Default.GetJsonSchemaAsNode(typeof(TResponse));
        schemaNode["type"] = "object";
        schemaNode["additionalProperties"] = false;
        return schemaNode.ToJsonString();
    }
}