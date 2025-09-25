namespace NexAI.Zendesk;

public record ZendeskTicket(
    ZendeskTicketId Id, 
    string ExternalId, 
    string Title, 
    string Description,
    string Url,
    string Category,
    string Status,
    string Country,
    string MerchantId,
    string[] Tags,
    DateTime CreatedAt, 
    DateTime? UpdatedAt, 
    ZendeskTicket.ZendeskTicketMessage[] Messages)
{
    public record ZendeskTicketMessage(ZendeskTicketMessageId Id, string ExternalId, string Content, string Author, DateTime CreatedAt);
}