using MongoDB.Driver;
using NexAI.Config;
using NexAI.MongoDb;

namespace NexAI.Zendesk.Queries;

public class GetZendeskTicketsByIdsQuery(Options options)
{
    private readonly MongoDbOptions _mongoDbOptions = options.Get<MongoDbOptions>();

    public async Task<ZendeskTicket[]> Handle(Guid[] ids)
    {
        var clientSettings = MongoClientSettings.FromUrl(new(_mongoDbOptions.ConnectionString));
        var client = new MongoClient(clientSettings);
        var database = client.GetDatabase(_mongoDbOptions.Database);
        var collection = database.GetCollection<ZendeskTicketMongoDbDocument>(ZendeskTicketCollections.MongoDbCollectionName);
        var filter = Builders<ZendeskTicketMongoDbDocument>.Filter.In(ticket => ticket.Id, ids);
        var zendeskTickets = await collection.Find(filter).ToListAsync();
        return zendeskTickets.Select(document => document.ToZendeskTicket()).ToArray();
    }
}