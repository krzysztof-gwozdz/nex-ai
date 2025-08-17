using MongoDB.Driver;
using NexAI.Config;
using NexAI.MongoDb;

namespace NexAI.Zendesk.Queries;

public class GetZendeskTicketByNumberQuery(Options options)
{
    private readonly MongoDbOptions _mongoDbOptions = options.Get<MongoDbOptions>();

    public async Task<ZendeskTicket?> Handle(string number)
    {
        var clientSettings = MongoClientSettings.FromUrl(new(_mongoDbOptions.ConnectionString));
        var client = new MongoClient(clientSettings);
        var database = client.GetDatabase(_mongoDbOptions.Database);
        var collection = database.GetCollection<ZendeskTicketMongoDbDocument>(ZendeskTicketCollections.MongoDbCollectionName);
        var filter = Builders<ZendeskTicketMongoDbDocument>.Filter.Eq(ticket => ticket.Number, number);
        var zendeskTicket = await collection.Find(filter).FirstOrDefaultAsync();
        return zendeskTicket.ToZendeskTicket();
    }
}