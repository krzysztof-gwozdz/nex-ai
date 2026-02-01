// ReSharper disable InconsistentNaming

using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using NexAI.Config;
using NexAI.MongoDb;
using NexAI.Neo4j;
using NexAI.Tests;
using NexAI.Zendesk.MongoDb;
using Xunit;

namespace NexAI.Zendesk.Tests;

public class TestBase : IAsyncLifetime
{
    private Neo4jTestContainer _neo4jTestContainer = null!;
    private MongoDbTestContainer _mongoDbTestContainer = null!;
    protected Neo4jDbClient Neo4jDbClient { get; private set; } = null!;
    protected MongoDbClient MongoDbClient { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _neo4jTestContainer = await Neo4jTestContainer.CreateStartedAsync();
        _mongoDbTestContainer = await MongoDbTestContainer.CreateStartedAsync();
        var settings = new Dictionary<string, string?>()
            .Concat(_neo4jTestContainer.Settings)
            .Concat(_mongoDbTestContainer.Settings);
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
        var options = new Options(configuration);
        Neo4jDbClient = new(options);
        MongoDbClient = new MongoDbClient(options);
    }

    public async Task DisposeAsync()
    {
        await Neo4jDbClient.CleanDatabase();
        var collection = MongoDbClient.GetCollection<ZendeskTicketMongoDbDocument>(ZendeskTicketMongoDbCollection.Name);
        await collection.DeleteManyAsync(FilterDefinition<ZendeskTicketMongoDbDocument>.Empty);
        Neo4jDbClient.Driver.Dispose();
        await _neo4jTestContainer.DisposeAsync();
        await _mongoDbTestContainer.DisposeAsync();
    }
}