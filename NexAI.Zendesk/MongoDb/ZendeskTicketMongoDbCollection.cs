using MongoDB.Driver;
using NexAI.MongoDb;

namespace NexAI.Zendesk.MongoDb;

public class ZendeskTicketMongoDbCollection(MongoDbClient mongoDbClient)
{
    public const string Name = "zendesk_tickets";

    public IMongoCollection<ZendeskTicketMongoDbDocument> Collection =>
        mongoDbClient.Database.GetCollection<ZendeskTicketMongoDbDocument>(Name);
}