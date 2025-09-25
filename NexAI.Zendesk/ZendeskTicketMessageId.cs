namespace NexAI.Zendesk;

public readonly record struct ZendeskTicketMessageId(Guid Value)
{
    public static ZendeskTicketMessageId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(ZendeskTicketMessageId id) => id.Value;
    
    public static implicit operator string(ZendeskTicketMessageId id) => id.Value.ToString();
}