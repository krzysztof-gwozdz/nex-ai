using MongoDB.Driver;
using NexAI.MongoDb;

namespace NexAI.Zendesk.Queries;

public class GetZendeskTicketsByIdsQuery(MongoDbClient mongoDbClient)
{
    public async Task<ZendeskTicket[]> Handle(Guid[] ids, CancellationToken cancellationToken)
    {
        var collection = mongoDbClient.GetCollection<ZendeskTicketMongoDbDocument>(ZendeskTicketCollections.MongoDbCollectionName);
        var filter = Builders<ZendeskTicketMongoDbDocument>.Filter.In(ticket => ticket.Id, ids);
        var zendeskTickets = await collection.Find(filter).ToListAsync(cancellationToken: cancellationToken);
        return zendeskTickets.Select(document => document.ToZendeskTicket()).ToArray();
    }
}