using MongoDB.Driver;
using NexAI.MongoDb;

namespace NexAI.Zendesk.Queries;

public class GetZendeskTicketsByExternalIdsQuery(MongoDbClient mongoDbClient)
{
    public async Task<ZendeskTicket[]> Handle(string[] externalIds)
    {
        var collection = mongoDbClient.GetCollection<ZendeskTicketMongoDbDocument>(ZendeskTicketCollections.MongoDbCollectionName);
        var filter = Builders<ZendeskTicketMongoDbDocument>.Filter.In(ticket => ticket.ExternalId, externalIds);
        var zendeskTickets = await collection.Find(filter).ToListAsync();
        return zendeskTickets.Select(document => document.ToZendeskTicket()).ToArray();
    }
}