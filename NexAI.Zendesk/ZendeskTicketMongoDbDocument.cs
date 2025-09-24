using MongoDB.Bson.Serialization.Attributes;

namespace NexAI.Zendesk;

public record ZendeskTicketMongoDbDocument
{
    private ZendeskTicketMongoDbDocument()
    {
    }

    public ZendeskTicketMongoDbDocument(ZendeskTicketId id, string number, string title, string description, string url, string category, string status, string country, string merchantId, string[] tags, DateTime createdAt, DateTime? updatedAt, MessageDocument[] messages) : this()
    {
        Id = id;
        Number = number;
        Title = title;
        Description = description;
        Url = url;
        Category = category;
        Status = status;
        Country = country;
        MerchantId = merchantId;
        Tags = tags;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        Messages = messages;
        FirstImportDate = DateTime.UtcNow;
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

    [BsonElement("url")]
    public string Url { get; init; } = string.Empty;

    [BsonElement("category")]
    public string Category { get; init; } = string.Empty;

    [BsonElement("status")]
    public string Status { get; init; } = string.Empty;

    [BsonElement("country")]
    public string Country { get; init; } = string.Empty;

    [BsonElement("merchantId")]
    public string MerchantId { get; init; } = string.Empty;

    [BsonElement("tags")]
    public string[] Tags { get; init; } = [];

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; init; }

    [BsonElement("updatedAt")]
    public DateTime? UpdatedAt { get; init; }

    [BsonElement("messages")]
    public MessageDocument[] Messages { get; init; } = [];
    
    [BsonElement("firstImportDate")]
    public DateTime FirstImportDate { get; init; }

    [BsonIgnore]
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
            Url,
            Category,
            Status,
            Country,
            MerchantId,
            Tags,
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
            zendeskTicket.Url,
            zendeskTicket.Category,
            zendeskTicket.Status,
            zendeskTicket.Country,
            zendeskTicket.MerchantId,
            zendeskTicket.Tags,
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