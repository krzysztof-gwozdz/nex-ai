using NexAI.Config;

namespace NexAI.Zendesk;

public readonly record struct ZendeskTicketMessageId
{
    public Guid Value { get; init; }

    public ZendeskTicketMessageId(Guid value)
    {
        ArgumentExceptionExtensions.ThrowIfNullOrEmpty(value);
        Value = value;
    }
    
    public static ZendeskTicketMessageId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(ZendeskTicketMessageId id) => id.Value;
    
    public static implicit operator string(ZendeskTicketMessageId id) => id.Value.ToString();
}