using MongoDB.Driver;
using NexAI.Zendesk.MongoDb;

namespace NexAI.Zendesk.Queries;

public class GetZendeskTicketsByExternalIdsQuery(ZendeskTicketMongoDbCollection zendeskTicketMongoDbCollection)
{
    public async Task<ZendeskTicket[]> Handle(string[] externalIds, CancellationToken cancellationToken)
    {
        var filter = Builders<ZendeskTicketMongoDbDocument>.Filter.In(ticket => ticket.ExternalId, externalIds);
        var zendeskTickets = await zendeskTicketMongoDbCollection.Collection.Find(filter).ToListAsync(cancellationToken: cancellationToken);
        return zendeskTickets.Select(document => document.ToZendeskTicket()).ToArray();
    }
}