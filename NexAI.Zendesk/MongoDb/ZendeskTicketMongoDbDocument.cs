using MongoDB.Bson.Serialization.Attributes;

namespace NexAI.Zendesk.MongoDb;

public record ZendeskTicketMongoDbDocument
{
    private ZendeskTicketMongoDbDocument()
    {
    }

    public ZendeskTicketMongoDbDocument(
        ZendeskTicketId id,
        string url,
        string externalId,
        string title,
        string description,
        string? mainCategory,
        string? category,
        string status,
        string? country,
        string? merchantId,
        string? level3Team,
        string[] tags,
        DateTime createdAt,
        DateTime? updatedAt,
        MessageDocument[] messages,
        DateTime firstImportDate,
        DateTime lastImportDate) : this()
    {
        Id = id;
        Url = url;
        ExternalId = externalId;
        Title = title;
        Description = description;
        MainCategory = mainCategory;
        Category = category;
        Status = status;
        Country = country;
        MerchantId = merchantId;
        Level3Team = level3Team;
        Tags = tags;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        Messages = messages;
        FirstImportDate = firstImportDate;
        LastImportDate = lastImportDate;
    }

    [BsonId]
    [BsonElement("_id")]
    public Guid Id { get; init; }

    [BsonElement("externalId")]
    public string ExternalId { get; init; } = string.Empty;

    [BsonElement("url")]
    public string Url { get; private set; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; private set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; private set; } = string.Empty;

    [BsonElement("mainCategory")]
    public string? MainCategory { get; private set; }

    [BsonElement("category")]
    public string? Category { get; private set; }

    [BsonElement("status")]
    public string Status { get; private set; } = string.Empty;

    [BsonElement("country")]
    public string? Country { get; private set; }

    [BsonElement("merchantId")]
    public string? MerchantId { get; private set; }
    
    [BsonElement("level3Team")]
    public string? Level3Team { get; private set; }

    [BsonElement("tags")]
    public string[] Tags { get; private set; } = [];

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; private set; }

    [BsonElement("updatedAt")]
    public DateTime? UpdatedAt { get; private set; }

    [BsonElement("messages")]
    public MessageDocument[] Messages { get; private set; } = [];

    [BsonElement("firstImportDate")]
    public DateTime FirstImportDate { get; private set; }

    [BsonElement("lastImportDate")]
    public DateTime LastImportDate { get; private set; }

    [BsonElement("score")]
    public double Score { get; private set; }

    public record MessageDocument
    {
        [BsonElement("id")]
        public Guid Id { get; init; }

        [BsonElement("externalId")]
        public string ExternalId { get; init; } = string.Empty;

        [BsonElement("content")]
        public string Content { get; private set; } = string.Empty;

        [BsonElement("author")]
        public string Author { get; private set; } = string.Empty;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; private set; }

        public MessageDocument()
        {
        }

        public MessageDocument(ZendeskTicketMessageId id, string externalId, string content, string author, DateTime createdAt) : this()
        {
            Id = id;
            ExternalId = externalId;
            Content = content;
            Author = author;
            CreatedAt = createdAt;
        }
    }

    public ZendeskTicket ToZendeskTicket() =>
        new(
            new(Id),
            ExternalId,
            Title,
            Description,
            Url,
            MainCategory,
            Category,
            Status,
            Country,
            MerchantId,
            Level3Team,
            Tags,
            CreatedAt,
            UpdatedAt,
            Messages.Select(message => new ZendeskTicket.ZendeskTicketMessage(new(message.Id), message.ExternalId, message.Content, message.Author, message.CreatedAt)).ToArray()
        );

    public static ZendeskTicketMongoDbDocument Create(ZendeskTicket zendeskTicket) =>
        new(
            zendeskTicket.Id,
            zendeskTicket.Url,
            zendeskTicket.ExternalId,
            zendeskTicket.Title,
            zendeskTicket.Description,
            zendeskTicket.MainCategory,
            zendeskTicket.Category,
            zendeskTicket.Status,
            zendeskTicket.Country,
            zendeskTicket.MerchantId,
            zendeskTicket.Level3Team,
            zendeskTicket.Tags,
            zendeskTicket.CreatedAt,
            zendeskTicket.UpdatedAt,
            zendeskTicket.Messages
                .Select(message =>
                    new MessageDocument(
                        message.Id,
                        message.ExternalId,
                        message.Content,
                        message.Author,
                        message.CreatedAt)
                ).ToArray(),
            DateTime.UtcNow,
            DateTime.UtcNow
        );

    public void Update(ZendeskTicket zendeskTicket)
    {
        Title = zendeskTicket.Title;
        Description = zendeskTicket.Description;
        Category = zendeskTicket.Category;
        Status = zendeskTicket.Status;
        Country = zendeskTicket.Country;
        MerchantId = zendeskTicket.MerchantId;
        Level3Team = zendeskTicket.Level3Team;
        Tags = zendeskTicket.Tags;
        UpdatedAt = zendeskTicket.UpdatedAt;
        Messages = zendeskTicket.Messages
            .Select(message =>
                new MessageDocument(
                    message.Id,
                    message.ExternalId,
                    message.Content,
                    message.Author,
                    message.CreatedAt)
            ).ToArray();
        LastImportDate = DateTime.UtcNow;
    }
}