using System.Runtime.CompilerServices;
using NexAI.LLMs.Common;
using zborek.Langfuse.OpenTelemetry.Models;
using zborek.Langfuse.OpenTelemetry.Trace;

namespace NexAI.LLMs.Langfuse;

public class LangfuseChatDecorator(Chat chat, IOtelLangfuseTrace langfuseTrace) : Chat
{
    public override string Provider => chat.Provider;
    public override string Model => chat.Model;
    
    public override async Task<string> Ask(ConversationId conversationId, string systemMessage, string message, CancellationToken cancellationToken)
    {
        using var trace = StartTrace(conversationId, ToMessages(systemMessage, message)); 
        var response = await chat.Ask(conversationId, systemMessage, message, cancellationToken);
        EndTrace(trace, response);
        return response;
    }

    public override async Task<TResponse> Ask<TResponse>(ConversationId conversationId, string systemMessage, string message, CancellationToken cancellationToken)
    {
        using var trace = StartTrace(conversationId, ToMessages(systemMessage, message)); 
        var response = await chat.Ask<TResponse>(conversationId, systemMessage, message, cancellationToken);
        EndTrace(trace, response);
        return response;
    }

    public override async IAsyncEnumerable<string> AskStream(ConversationId conversationId, string systemMessage, string message, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var trace = StartTrace(conversationId, ToMessages(systemMessage, message)); 
        var fullResponse = new List<string>();
        await foreach (var chunk in chat.AskStream(conversationId, systemMessage, message, cancellationToken))
        {
            fullResponse.Add(chunk);
            yield return chunk;
        }
        var response = string.Concat(fullResponse);
        EndTrace(trace, response);
    }

    public override async Task<string> GetNextResponse(ConversationId conversationId, ChatMessage[] messages, CancellationToken cancellationToken)
    {
        using var trace = StartTrace(conversationId, messages); 
        var response = await chat.GetNextResponse(conversationId, messages, cancellationToken);
        EndTrace(trace, response);
        return response;
    }

    public override async IAsyncEnumerable<string> StreamNextResponse(ConversationId conversationId, ChatMessage[] messages, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var trace = StartTrace(conversationId, messages); 
        var fullResponse = new List<string>();
        await foreach (var chunk in chat.StreamNextResponse(conversationId, messages, cancellationToken))
        {
            fullResponse.Add(chunk);
            yield return chunk;
        }
        var response = string.Concat(fullResponse);
        EndTrace(trace, response);
    }

    private OtelGeneration StartTrace(ConversationId conversationId, ChatMessage[] messages)
    {
        langfuseTrace.StartTrace("nexai-chat", input: new { messages }, tags: [conversationId]);
        return langfuseTrace.CreateGeneration(
            "chat",
            model: Model,
            provider: Provider,
            input: new { messages },
            configure: otelGeneration =>
                otelGeneration.SetInputMessages(messages.Select(message => new GenAiMessage { Role = message.Role, Content = message.Content })
                    .ToList()));
    }
    
    private void EndTrace<TResponse>(OtelGeneration openTelemetryGeneration, TResponse response)
    {
        langfuseTrace.SetOutput(new { response });
        openTelemetryGeneration.Dispose();
    }
}