using NexAI.Config;

namespace NexAI.Zendesk;

public readonly record struct ZendeskGroupId
{
    public Guid Value { get; init; }

    public ZendeskGroupId(Guid value)
    {
        ArgumentExceptionExtensions.ThrowIfNullOrEmpty(value);
        Value = value;
    }

    public static ZendeskGroupId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(ZendeskGroupId id) => id.Value;

    public static implicit operator string(ZendeskGroupId id) => id.Value.ToString();
}