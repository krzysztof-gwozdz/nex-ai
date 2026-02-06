using System.Runtime.CompilerServices;
using NexAI.LLMs.Common;

namespace NexAI.Agents;

sealed class FakeNexAIAgent(Chat chat) : INexAIAgent
{
    private ChatMessage[]? _messages = [];
    
    public void StartNewChat(ConversationId conversationId, ChatMessage[]? messages = null) => 
        _messages = messages;

    public Task<string> GetResponse(ConversationId conversationId, CancellationToken cancellationToken) =>
        chat.GetNextResponse(conversationId, _messages ?? [], cancellationToken);

    public async IAsyncEnumerable<string> StreamResponse(ConversationId conversationId, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var chunks = (await chat.GetNextResponse(conversationId, _messages ?? [], cancellationToken)).Split(' ');
        foreach (var chunk in chunks)
        {
            yield return chunk;
            if (chunk != chunks[^1])
                yield return " ";
        }
    }
}
