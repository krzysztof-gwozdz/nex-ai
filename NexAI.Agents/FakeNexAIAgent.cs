using System.Runtime.CompilerServices;
using NexAI.LLMs.Common;
using NexAI.LLMs.Fake;

namespace NexAI.Agents;

sealed class FakeNexAIAgent() : INexAIAgent
{
    private readonly FakeChat _chat = new();
    private ChatMessage[]? _messages = [];
    
    public void StartNewChat(ChatMessage[]? messages = null) => 
        _messages = messages;

    public Task<string> GetResponse(CancellationToken cancellationToken) =>
        _chat.GetNextResponse(_messages ?? [], cancellationToken);

    public async IAsyncEnumerable<string> StreamResponse([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var chunks = (await _chat.GetNextResponse(_messages ?? [], cancellationToken)).Split(' ');
        foreach (var chunk in chunks)
        {
            yield return chunk;
            if (chunk != chunks[^1])
                yield return " ";
        }
    }
}
