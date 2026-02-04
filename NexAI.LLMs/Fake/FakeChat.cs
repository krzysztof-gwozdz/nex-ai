using System.Runtime.CompilerServices;
using NexAI.LLMs.Common;

namespace NexAI.LLMs.Fake;

public class FakeChat : Chat
{
    private readonly Random _random = new();

    private readonly List<string> _jokes =
    [
        "Why don't skeletons fight each other? They don't have the guts!",
        "What do you call a fake noodle? An impasta.",
        "What do you call a bear with no teeth? A gummy bear.",
        "What do you call a fish with no eyes? A fsh.",
        "What do you call a fake noodle? An impasta.",
        "What do you call a bear with no teeth? A gummy bear.",
        "What do you call a fish with no eyes? A fsh.",
    ];

    public override Task<string> Ask(ConversationId conversationId, string systemMessage, string message, CancellationToken cancellationToken) => 
        Task.FromResult(GetResponse(systemMessage, message));

    public override Task<TResponse> Ask<TResponse>(ConversationId conversationId, string systemMessage, string message, CancellationToken cancellationToken) => 
        throw new NotSupportedException();

    public override async IAsyncEnumerable<string> AskStream(ConversationId conversationId, string systemMessage, string message, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var chunks = GetResponse(systemMessage, message).Split(' ');
        foreach (var chunk in chunks)
        {
            yield return chunk;
            if (chunk != chunks[^1])
                yield return " ";
        }
    }

    public override Task<string> GetNextResponse(ConversationId conversationId, ChatMessage[] messages, CancellationToken cancellationToken) =>
        Task.FromResult(GetResponse(messages.FirstOrDefault(message => message.Role == "system")?.Content, messages.LastOrDefault(message => message.Role == "user")?.Content));

    public override async IAsyncEnumerable<string> StreamNextResponse(ConversationId conversationId, ChatMessage[] messages, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var chunks = GetResponse(messages.FirstOrDefault(message => message.Role == "system")?.Content, messages.LastOrDefault(message => message.Role == "user")?.Content).Split(' ');
        foreach (var chunk in chunks)
        {
            yield return chunk;
            if (chunk != chunks[^1])
                yield return " ";
        }
    }

    private string GetResponse(string? systemMessage, string? message)
    {
        if (systemMessage?.Contains("TEST") ?? false)
        {
            return "TEST";
        }
        if (message?.Contains("ping") ?? false)
        {
            return "pong";
        }
        if (message?.Contains("joke") ?? false)
        {
            return _jokes[_random.Next(_jokes.Count)];
        }
        if ((systemMessage?.Contains("summary") ?? false) || (systemMessage?.Contains("summarize") ?? false )
            || (message?.Contains("summary") ?? false) || (message?.Contains("summarize") ?? false))
        {
            return "This super valid summary of super exciting ticket.";
        }
        return "I'm sorry, I can't help with that.";
    }
}