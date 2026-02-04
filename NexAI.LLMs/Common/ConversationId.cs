using NexAI.Config;

namespace NexAI.LLMs.Common;

public readonly record struct ConversationId
{
    public Guid Value { get; init; }

    public ConversationId(Guid value)
    {
        ArgumentExceptionExtensions.ThrowIfNullOrEmpty(value);
        Value = value;
    }

    public static ConversationId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(ConversationId id) => id.Value;

    public static implicit operator string(ConversationId id) => id.Value.ToString();
}