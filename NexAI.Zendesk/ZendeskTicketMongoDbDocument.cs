using MongoDB.Bson.Serialization.Attributes;

namespace NexAI.Zendesk;

public record ZendeskTicketMongoDbDocument
{
    [BsonId]
    [BsonElement("_id")]
    public Guid Id { get; init; }

    [BsonElement("number")]
    public string Number { get; init; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; init; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; init; } = string.Empty;

    [BsonElement("messages")]
    public MessageDocument[] Messages { get; init; } = [];

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
            Id,
            Number,
            Title,
            Description,
            Messages.Select(message => new ZendeskTicket.ZendeskTicketMessage(message.Content, message.Author, message.CreatedAt)).ToArray()
        );
}