namespace NexAI.Zendesk;

public record ZendeskTicket(
    ZendeskTicketId Id, 
    string Number, 
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
    public record ZendeskTicketMessage(string Content, string Author, DateTime CreatedAt);
}