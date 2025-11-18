namespace NexAI.Zendesk.Messages;

public record ZendeskTicketImportedEvent(
    Guid Id,
    string ExternalId,
    string Title,
    string Description,
    string Url,
    string? MainCategory,
    string? Category,
    string Status,
    string? Country,
    string? MerchantId,
    string? Level3Team,
    string[] Tags,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    ZendeskTicketImportedEvent.ZendeskTicketMessage[] Messages)
{
    public record ZendeskTicketMessage(Guid Id, string ExternalId, string Content, string Author, DateTime CreatedAt) : IEvent;
}