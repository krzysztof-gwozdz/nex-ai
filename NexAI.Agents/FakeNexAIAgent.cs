using System.Runtime.CompilerServices;
using NexAI.LLMs.Common;
using NexAI.LLMs.Fake;

namespace NexAI.Agents;

sealed class FakeNexAIAgent() : INexAIAgent
{
    private readonly FakeChat _chat = new();
    private ChatMessage[]? _messages = [];
    
    public void StartNewChat(ConversationId conversationId, ChatMessage[]? messages = null) => 
        _messages = messages;

    public Task<string> GetResponse(ConversationId conversationId, CancellationToken cancellationToken) =>
        _chat.GetNextResponse(conversationId, _messages ?? [], cancellationToken);

    public async IAsyncEnumerable<string> StreamResponse(ConversationId conversationId, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var chunks = (await _chat.GetNextResponse(conversationId, _messages ?? [], cancellationToken)).Split(' ');
        foreach (var chunk in chunks)
        {
            yield return chunk;
            if (chunk != chunks[^1])
                yield return " ";
        }
    }
}
