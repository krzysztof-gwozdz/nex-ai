namespace NexAI.Zendesk;

public record ZendeskTicket(
    ZendeskTicketId Id,
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
    ZendeskTicket.ZendeskTicketMessage[] Messages)
{
    public record ZendeskTicketMessage(ZendeskTicketMessageId Id, string ExternalId, string Content, string Author, DateTime CreatedAt);

    public static ZendeskTicket Create
    (
        string externalId,
        string title,
        string description,
        string url,
        string? category,
        string status,
        string? country,
        string? merchantId,
        string[] tags,
        DateTime createdAt,
        DateTime? updatedAt,
        ZendeskTicketMessage[] messages
    ) =>
        new(
            ZendeskTicketId.New(),
            externalId,
            title,
            description,
            url,
            GetMainCategory(category),
            category,
            status,
            country,
            merchantId,
            GetLevel3Team(tags),
            tags,
            createdAt,
            updatedAt,
            messages);

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

    private static string? GetMainCategory(string? category) =>
        category?.Split("__").FirstOrDefault();

    private static string? GetLevel3Team(string[] tags) =>
        tags.FirstOrDefault(tag => tag.StartsWith("team_l3_"))?.Replace("team_l3_", "");
}