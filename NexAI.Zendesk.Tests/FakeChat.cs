using System.Runtime.CompilerServices;
using NexAI.LLMs.Common;

namespace NexAI.Zendesk.Tests;

sealed class FakeChat(string fixedResponse, params ChatMessage[] messages) : Chat
{
    public override string Provider => "Fake";
    public override string Model => "Fake";
    
    public List<ChatMessage> Messages { get; } = messages.ToList();

    public override Task<string> Ask(ConversationId conversationId, string systemMessage, string message, CancellationToken cancellationToken)
    {
        Messages.Add(new ChatMessage("system", systemMessage));
        Messages.Add(new ChatMessage("user", message));
        return Task.FromResult(fixedResponse);
    }

    public override Task<TResponse> Ask<TResponse>(ConversationId conversationId, string systemMessage, string message, CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public override async IAsyncEnumerable<string> AskStream(ConversationId conversationId, string systemMessage, string message, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Messages.Add(new ChatMessage("system", systemMessage));
        Messages.Add(new ChatMessage("user", message));
        yield return fixedResponse;
    }

    public override Task<string> GetNextResponse(ConversationId conversationId, ChatMessage[] messages, CancellationToken cancellationToken) =>
        Task.FromResult(fixedResponse);

    public override IAsyncEnumerable<string> StreamNextResponse(ConversationId conversationId, ChatMessage[] messages, CancellationToken cancellationToken) =>
        throw new NotSupportedException();
}