using System.Text.Json;
using System.Text.Json.Schema;

namespace NexAI.LLMs.Common;

public abstract class Chat
{
    public abstract Task<string> Ask(string systemMessage, string message);

    public abstract Task<TResponse> Ask<TResponse>(string systemMessage, string message);

    public abstract IAsyncEnumerable<string> AskStream(string systemMessage, string message);

    protected static string GetSchema<TResponse>()
    {
        var schemaNode = JsonSerializerOptions.Default.GetJsonSchemaAsNode(typeof(TResponse));
        schemaNode["type"] = "object";
        schemaNode["additionalProperties"] = false;
        return schemaNode.ToJsonString();
    }
}