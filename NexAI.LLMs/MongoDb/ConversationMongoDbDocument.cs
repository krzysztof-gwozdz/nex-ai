using MongoDB.Bson.Serialization.Attributes;
using NexAI.LLMs.Common;

namespace NexAI.LLMs.MongoDb;

public record ConversationMongoDbDocument
{
    [BsonId] 
    [BsonElement("_id")] 
    public Guid Id { get; init; }

    [BsonElement("messages")] 
    public MessageMongoDbDocument[] Messages { get; init; } = [];

    public record MessageMongoDbDocument
    {
        [BsonElement("role")] 
        public string Role { get; init; } = string.Empty;

        [BsonElement("content")] 
        public string Content { get; init; } = string.Empty;

        private MessageMongoDbDocument()
        {
        }

        private MessageMongoDbDocument(string role, string content) : this()
        {
            Role = role;
            Content = content;
        }

        public static MessageMongoDbDocument Create(ChatMessage message) =>
            new(message.Role, message.Content);
    }

    private ConversationMongoDbDocument()
    {
    }

    public ConversationMongoDbDocument(Guid id, MessageMongoDbDocument[] messages) : this()
    {
        Id = id;
        Messages = messages;
    }

    public static ConversationMongoDbDocument Create(Conversation conversation) =>
        new(conversation.Id, conversation.Messages.Select(MessageMongoDbDocument.Create).ToArray());
}