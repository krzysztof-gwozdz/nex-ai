using MongoDB.Driver;
using NexAI.Config;
using NexAI.MongoDb;

namespace NexAI.Zendesk.Queries;

public class FindZendeskTicketsThatContainPhraseQuery(Options options)
{
    private readonly MongoDbOptions _mongoDbOptions = options.Get<MongoDbOptions>();

    public async Task<ZendeskTicket[]> Handle(string phrase, int limit)
    {
        var clientSettings = MongoClientSettings.FromUrl(new(_mongoDbOptions.ConnectionString));
        var client = new MongoClient(clientSettings);
        var database = client.GetDatabase(_mongoDbOptions.Database);
        var collection = database.GetCollection<ZendeskTicketMongoDbDocument>(ZendeskTicketCollections.MongoDbCollectionName);
        var filter = Builders<ZendeskTicketMongoDbDocument>.Filter.Text(phrase);
        var results = await collection.Find(filter).Limit(limit).ToListAsync();
        return results.Select(document => document.ToZendeskTicket()).ToArray();
    }
}