namespace NexAI.Zendesk;

public readonly record struct ZendeskTicketId
{
    public Guid Value { get; init; }

    public ZendeskTicketId(Guid value)
    {
        ArgumentExceptionExtensions.ThrowIfNullOrEmpty(value);
        Value = value;
    }

    public static ZendeskTicketId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(ZendeskTicketId id) => id.Value;

    public static implicit operator string(ZendeskTicketId id) => id.Value.ToString();
}