using MongoDB.Driver;
using NexAI.MongoDb;

namespace NexAI.LLMs.MongoDb;

public class ConversationMongoDbCollection(MongoDbClient mongoDbClient)
{
    public const string Name = "conversations";

    public IMongoCollection<ConversationMongoDbDocument> Collection =>
        mongoDbClient.Database.GetCollection<ConversationMongoDbDocument>(Name);
}
