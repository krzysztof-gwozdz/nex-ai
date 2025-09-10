using MongoDB.Bson.Serialization.Attributes;

namespace NexAI.Zendesk;

public record ZendeskTicketMongoDbDocument
{
    private ZendeskTicketMongoDbDocument()
    {
    }

    public ZendeskTicketMongoDbDocument(ZendeskTicketId id, string number, string title, string description, DateTime createdAt, DateTime? updatedAt, MessageDocument[] messages) : this()
    {
        Id = id;
        Number = number;
        Title = title;
        Description = description;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        Messages = messages;
    }

    [BsonId]
    [BsonElement("_id")]
    public Guid Id { get; init; }

    [BsonElement("number")]
    public string Number { get; init; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; init; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; init; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; init; }

    [BsonElement("updatedAt")]
    public DateTime? UpdatedAt { get; init; }

    [BsonElement("messages")]
    public MessageDocument[] Messages { get; init; } = [];

    [BsonElement("score")]
    public double Score { get; init; }

    public record MessageDocument
    {
        [BsonElement("content")]
        public string Content { get; init; } = string.Empty;

        [BsonElement("author")]
        public string Author { get; init; } = string.Empty;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; init; }
    }

    public ZendeskTicket ToZendeskTicket() =>
        new(
            new(Id),
            Number,
            Title,
            Description,
            CreatedAt,
            UpdatedAt,
            Messages.Select(message => new ZendeskTicket.ZendeskTicketMessage(message.Content, message.Author, message.CreatedAt)).ToArray()
        );

    public static ZendeskTicketMongoDbDocument Create(ZendeskTicket zendeskTicket) =>
        new(
            zendeskTicket.Id,
            zendeskTicket.Number,
            zendeskTicket.Title,
            zendeskTicket.Description,
            zendeskTicket.CreatedAt,
            zendeskTicket.UpdatedAt,
            zendeskTicket.Messages.Select(message => new MessageDocument
            {
                Content = message.Content,
                Author = message.Author,
                CreatedAt = message.CreatedAt
            }).ToArray()
        );
}