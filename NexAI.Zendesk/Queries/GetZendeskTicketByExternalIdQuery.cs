using MongoDB.Driver;
using NexAI.Zendesk.MongoDb;

namespace NexAI.Zendesk.Queries;

public class GetZendeskTicketByExternalIdQuery(ZendeskTicketMongoDbCollection zendeskTicketMongoDbCollection)
{
    public async Task<ZendeskTicket?> Handle(string externalId, CancellationToken cancellationToken)
    {
        var filter = Builders<ZendeskTicketMongoDbDocument>.Filter.Eq(ticket => ticket.ExternalId, externalId);
        var zendeskTicket = await zendeskTicketMongoDbCollection.Collection.Find(filter).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        return zendeskTicket?.ToZendeskTicket();
    }
}