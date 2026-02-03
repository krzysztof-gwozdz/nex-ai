using NexAI.LLMs.Common;

namespace NexAI.Agents;

public interface INexAIAgent
{
    void StartNewChat(ChatMessage[]? messages = null);

    Task<string> GetResponse(CancellationToken cancellationToken);

    IAsyncEnumerable<string> StreamResponse(CancellationToken cancellationToken);
}
