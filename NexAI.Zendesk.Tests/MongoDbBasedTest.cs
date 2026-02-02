using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using NexAI.Config;
using NexAI.MongoDb;
using NexAI.Tests;
using NexAI.Zendesk.MongoDb;
using Xunit;

namespace NexAI.Zendesk.Tests;

public class MongoDbBasedTest : IAsyncLifetime
{
    private MongoDbTestContainer _mongoDbTestContainer = null!;
    protected MongoDbClient MongoDbClient { get; private set; } = null!;
    protected ZendeskTicketMongoDbCollection ZendeskTicketMongoDbCollection { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _mongoDbTestContainer = await MongoDbTestContainer.CreateStartedAsync();
        var settings = new Dictionary<string, string?>()
            .Concat(_mongoDbTestContainer.Settings);
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
        var options = new Options(configuration);
        MongoDbClient = new MongoDbClient(options);
        ZendeskTicketMongoDbCollection = new ZendeskTicketMongoDbCollection(MongoDbClient);
    }

    public async Task DisposeAsync()
    {
        await ZendeskTicketMongoDbCollection.Collection.DeleteManyAsync(FilterDefinition<ZendeskTicketMongoDbDocument>.Empty);
        await _mongoDbTestContainer.DisposeAsync();
    }
}