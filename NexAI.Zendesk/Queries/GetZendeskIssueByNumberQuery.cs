using MongoDB.Driver;
using NexAI.Config;
using NexAI.MongoDb;

namespace NexAI.Zendesk.Queries;

public class GetZendeskIssueByNumberQuery(Options options)
{
    private readonly MongoDbOptions _mongoDbOptions = options.Get<MongoDbOptions>();

    public async Task<ZendeskIssue?> Handle(string number)
    {
        var clientSettings = MongoClientSettings.FromUrl(new(_mongoDbOptions.ConnectionString));
        var client = new MongoClient(clientSettings);
        var database = client.GetDatabase(_mongoDbOptions.Database);
        var collection = database.GetCollection<ZendeskIssue>(ZendeskIssueCollections.MongoDbCollectionName);
        var filter = Builders<ZendeskIssue>.Filter.Eq(issue => issue.Number, number);
        var zendeskIssue = await collection.Find(filter).FirstOrDefaultAsync();
        return zendeskIssue;
    }
}