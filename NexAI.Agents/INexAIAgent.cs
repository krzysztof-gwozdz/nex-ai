using NexAI.LLMs.Common;

namespace NexAI.Agents;

public interface INexAIAgent
{
    void StartNewChat(ConversationId conversationId, ChatMessage[]? messages = null);

    Task<string> GetResponse(ConversationId conversationId, CancellationToken cancellationToken);

    IAsyncEnumerable<string> StreamResponse(ConversationId conversationId, CancellationToken cancellationToken);
}
