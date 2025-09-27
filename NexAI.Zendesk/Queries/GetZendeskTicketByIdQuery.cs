using MongoDB.Driver;
using NexAI.MongoDb;

namespace NexAI.Zendesk.Queries;

public class GetZendeskTicketsByIdQuery(MongoDbClient mongoDbClient)
{
    public async Task<ZendeskTicket?> Handle(Guid id)
    {
        var collection = mongoDbClient.GetCollection<ZendeskTicketMongoDbDocument>(ZendeskTicketCollections.MongoDbCollectionName);
        var filter = Builders<ZendeskTicketMongoDbDocument>.Filter.Eq(ticket => ticket.Id, id);
        var zendeskTicket = await collection.Find(filter).FirstOrDefaultAsync();
        return zendeskTicket?.ToZendeskTicket();
    }
}