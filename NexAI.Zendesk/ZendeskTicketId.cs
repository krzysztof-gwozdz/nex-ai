namespace NexAI.Zendesk;

public readonly record struct ZendeskTicketId(Guid Value)
{
    public static ZendeskTicketId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
    
    public static implicit operator Guid(ZendeskTicketId id) => id.Value;
}