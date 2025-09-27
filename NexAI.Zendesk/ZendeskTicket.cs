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

    public bool IsRelevant =>
        !(Messages.Length == 1 ||
          (
              Messages.Length == 2 &&
              (
                  Status is "closed" or "solved" ||
                  Title.StartsWith("Incoming call") ||
                  Title.StartsWith("Du har mottatt innsigelse") ||
                  Title.StartsWith("Ni har fått en invändning") ||
                  Title.StartsWith("Vi har fått en invändning")
              )
          ) ||
          Title.StartsWith("Sinch call answered on") ||
          Title.StartsWith("Escalated dispute with KlarnaDisputeId"));
}