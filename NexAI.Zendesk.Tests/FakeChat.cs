using System.Runtime.CompilerServices;
using NexAI.LLMs.Common;

namespace NexAI.Zendesk.Tests;

sealed class FakeChat(string fixedResponse, params ChatMessage[] messages) : Chat
{
    public List<ChatMessage> Messages { get; } = messages.ToList();

    public override Task<string> Ask(string systemMessage, string message, CancellationToken cancellationToken)
    {
        Messages.Add(new ChatMessage("system", systemMessage));
        Messages.Add(new ChatMessage("user", message));
        return Task.FromResult(fixedResponse);
    }

    public override Task<TResponse> Ask<TResponse>(string systemMessage, string message, CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public override async IAsyncEnumerable<string> AskStream(string systemMessage, string message, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Messages.Add(new ChatMessage("system", systemMessage));
        Messages.Add(new ChatMessage("user", message));
        yield return fixedResponse;
    }

    public override Task<string> GetNextResponse(ChatMessage[] messages, CancellationToken cancellationToken) =>
        Task.FromResult(fixedResponse);

    public override IAsyncEnumerable<string> StreamNextResponse(ChatMessage[] messages, CancellationToken cancellationToken) =>
        throw new NotSupportedException();
}