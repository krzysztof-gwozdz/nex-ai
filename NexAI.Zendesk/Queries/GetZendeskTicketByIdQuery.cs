using MongoDB.Driver;
using NexAI.Zendesk.MongoDb;

namespace NexAI.Zendesk.Queries;

public class GetZendeskTicketsByIdQuery(ZendeskTicketMongoDbCollection zendeskTicketMongoDbCollection)
{
    public async Task<ZendeskTicket?> Handle(Guid id, CancellationToken cancellationToken)
    {
        var filter = Builders<ZendeskTicketMongoDbDocument>.Filter.Eq(ticket => ticket.Id, id);
        var zendeskTicket = await zendeskTicketMongoDbCollection.Collection.Find(filter).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        return zendeskTicket?.ToZendeskTicket();
    }
}