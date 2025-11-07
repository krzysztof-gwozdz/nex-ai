using NexAI.Config;

namespace NexAI.Zendesk;

public readonly record struct ZendeskUserId
{
    public Guid Value { get; init; }

    public ZendeskUserId(Guid value)
    {
        ArgumentExceptionExtensions.ThrowIfNullOrEmpty(value);
        Value = value;
    }

    public static ZendeskUserId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(ZendeskUserId id) => id.Value;

    public static implicit operator string(ZendeskUserId id) => id.Value.ToString();
}