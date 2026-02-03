using System.Runtime.CompilerServices;
using NexAI.Agents;
using NexAI.LLMs.Common;

namespace NexAI.Api.Tests.Fakes;

sealed class FakeNexAIAgent(string fixedResponse = "Fake agent response") : INexAIAgent
{
    public void StartNewChat(ChatMessage[]? messages = null)
    {
    }

    public Task<string> GetResponse(CancellationToken cancellationToken) =>
        Task.FromResult(fixedResponse);

    public async IAsyncEnumerable<string> StreamResponse([EnumeratorCancellation] CancellationToken cancellationToken)
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
