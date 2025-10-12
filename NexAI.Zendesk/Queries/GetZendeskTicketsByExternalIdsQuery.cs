using MongoDB.Driver;
using NexAI.MongoDb;
using NexAI.Zendesk.MongoDb;

namespace NexAI.Zendesk.Queries;

public class GetZendeskTicketsByExternalIdsQuery(MongoDbClient mongoDbClient)
{
    public async Task<ZendeskTicket[]> Handle(string[] externalIds, CancellationToken cancellationToken)
    {
        var collection = mongoDbClient.GetCollection<ZendeskTicketMongoDbDocument>(ZendeskTicketMongoDbCollection.Name);
        var filter = Builders<ZendeskTicketMongoDbDocument>.Filter.In(ticket => ticket.ExternalId, externalIds);
        var zendeskTickets = await collection.Find(filter).ToListAsync(cancellationToken: cancellationToken);
        return zendeskTickets.Select(document => document.ToZendeskTicket()).ToArray();
    }
}