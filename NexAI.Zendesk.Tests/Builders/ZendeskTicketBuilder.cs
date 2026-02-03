namespace NexAI.Zendesk.Tests.Builders;

public class ZendeskTicketBuilder
{
    private string _externalId = Guid.NewGuid().ToString();
    private string _title = "Title";
    private string _description = "Description";

    private ZendeskTicketBuilder()
    {
    }

    public static ZendeskTicketBuilder Create() => new();

    public ZendeskTicketBuilder WithExternalId(string externalId)
    {
        _externalId = externalId;
        return this;
    }

    public ZendeskTicketBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public ZendeskTicketBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public ZendeskTicket Build()
    {
        var id = ZendeskTicketId.New();
        var createdAt = new DateTime(2025, 2, 1, 10, 0, 0, DateTimeKind.Utc);
        var message = new ZendeskTicket.ZendeskTicketMessage(
            ZendeskTicketMessageId.New(),
            "msg-ext-1",
            "Content",
            "Author",
            createdAt);
        return new ZendeskTicket(
            id,
            _externalId,
            _title,
            _description,
            "https://example.com/ticket/1",
            null,
            null,
            "open",
            null,
            null,
            null,
            [],
            createdAt,
            null,
            [message]);
    }
}
