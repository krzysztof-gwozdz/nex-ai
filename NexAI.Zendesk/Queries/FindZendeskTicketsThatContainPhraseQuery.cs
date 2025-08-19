using MongoDB.Driver;
using NexAI.Config;
using NexAI.MongoDb;

namespace NexAI.Zendesk.Queries;

public class FindZendeskTicketsThatContainPhraseQuery(Options options)
{
    private readonly MongoDbOptions _mongoDbOptions = options.Get<MongoDbOptions>();

    public async Task<Dictionary<ZendeskTicket, double>> Handle(string phrase, int limit)
    {
        var clientSettings = MongoClientSettings.FromUrl(new(_mongoDbOptions.ConnectionString));
        var client = new MongoClient(clientSettings);
        var database = client.GetDatabase(_mongoDbOptions.Database);
        var collection = database.GetCollection<ZendeskTicketMongoDbDocument>(ZendeskTicketCollections.MongoDbCollectionName);
        var filter = Builders<ZendeskTicketMongoDbDocument>.Filter.Text(phrase);

        var projection = Builders<ZendeskTicketMongoDbDocument>.Projection
            .MetaTextScore("score");

        var sort = Builders<ZendeskTicketMongoDbDocument>.Sort
            .MetaTextScore("score");

        var results = await collection
            .Find(filter)
            .Limit(limit)
            .Project<ZendeskTicketMongoDbDocument>(projection)
            .Sort(sort)
            .ToListAsync();
        
        return results.Select(document => (document.ToZendeskTicket(), document.Score)).ToDictionary(x => x.Item1, x => x.Item2);
    }
}