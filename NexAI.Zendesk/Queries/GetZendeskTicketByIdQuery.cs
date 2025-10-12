using MongoDB.Driver;
using NexAI.MongoDb;
using NexAI.Zendesk.MongoDb;

namespace NexAI.Zendesk.Queries;

public class GetZendeskTicketsByIdQuery(MongoDbClient mongoDbClient)
{
    public async Task<ZendeskTicket?> Handle(Guid id, CancellationToken cancellationToken)
    {
        var collection = mongoDbClient.GetCollection<ZendeskTicketMongoDbDocument>(ZendeskTicketMongoDbCollection.Name);
        var filter = Builders<ZendeskTicketMongoDbDocument>.Filter.Eq(ticket => ticket.Id, id);
        var zendeskTicket = await collection.Find(filter).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        return zendeskTicket?.ToZendeskTicket();
    }
}