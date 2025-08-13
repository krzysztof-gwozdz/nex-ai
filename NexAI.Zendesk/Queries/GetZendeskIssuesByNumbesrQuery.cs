using MongoDB.Driver;
using NexAI.Config;
using NexAI.MongoDb;

namespace NexAI.Zendesk.Queries;

public class GetZendeskIssuesByNumbersQuery(Options options)
{
    private readonly MongoDbOptions _mongoDbOptions = options.Get<MongoDbOptions>();

    public async Task<ZendeskIssue[]> Handle(string[] numbers)
    {
        var clientSettings = MongoClientSettings.FromUrl(new(_mongoDbOptions.ConnectionString));
        var client = new MongoClient(clientSettings);
        var database = client.GetDatabase(_mongoDbOptions.Database);
        var collection = database.GetCollection<ZendeskIssueMongoDbDocument>(ZendeskIssueCollections.MongoDbCollectionName);
        var filter = Builders<ZendeskIssueMongoDbDocument>.Filter.In(issue => issue.Number, numbers);
        var zendeskIssues = await collection.Find(filter).ToListAsync();
        return zendeskIssues.Select(document => document.ToZendeskIssue()).ToArray();
    }
}