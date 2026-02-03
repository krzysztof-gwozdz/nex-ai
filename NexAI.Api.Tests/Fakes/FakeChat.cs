using System.Runtime.CompilerServices;
using NexAI.LLMs.Common;

namespace NexAI.Api.Tests.Fakes;

sealed class FakeChat(string fixedResponse = "Fake chat response") : Chat
{
    public override Task<string> Ask(string systemMessage, string message, CancellationToken cancellationToken) =>
        Task.FromResult(fixedResponse);

    public override Task<TResponse> Ask<TResponse>(string systemMessage, string message,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public override async IAsyncEnumerable<string> AskStream(string systemMessage, string message, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var chunks = fixedResponse.Split(' ');
        foreach (var chunk in chunks)
        {
            yield return chunk;
            if (chunk != chunks[^1])
                yield return " ";
        }
    }

    public override Task<string> GetNextResponse(ChatMessage[] messages, CancellationToken cancellationToken) =>
        Task.FromResult(fixedResponse);

    public override async IAsyncEnumerable<string> StreamNextResponse(ChatMessage[] messages, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var chunks = fixedResponse.Split(' ');
        foreach (var chunk in chunks)
        {
            yield return chunk;
            if (chunk != chunks[^1])
                yield return " ";
        }
    }
}