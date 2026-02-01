using MongoDB.Driver;
using NexAI.Zendesk.MongoDb;

namespace NexAI.Zendesk.Queries;

public class GetZendeskTicketsByIdsQuery(ZendeskTicketMongoDbCollection zendeskTicketMongoDbCollection)
{
    public async Task<ZendeskTicket[]> Handle(Guid[] ids, CancellationToken cancellationToken)
    {
        var filter = Builders<ZendeskTicketMongoDbDocument>.Filter.In(ticket => ticket.Id, ids);
        var zendeskTickets = await zendeskTicketMongoDbCollection.Collection.Find(filter).ToListAsync(cancellationToken: cancellationToken);
        return zendeskTickets.Select(document => document.ToZendeskTicket()).ToArray();
    }
}