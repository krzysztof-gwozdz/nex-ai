// ReSharper disable InconsistentNaming

using Microsoft.Extensions.Configuration;
using NexAI.Config;
using NexAI.Neo4j;
using NexAI.Tests;
using Xunit;

namespace NexAI.Zendesk.Tests;

public class Neo4jDbBasedTest : IAsyncLifetime
{
    private Neo4jTestContainer _neo4jTestContainer = null!;
    protected Neo4jDbClient Neo4jDbClient { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _neo4jTestContainer = await Neo4jTestContainer.CreateStartedAsync();
        var settings = new Dictionary<string, string?>()
            .Concat(_neo4jTestContainer.Settings);
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
        var options = new Options(configuration);
        Neo4jDbClient = new(options);
    }

    public async Task DisposeAsync()
    {
        await Neo4jDbClient.CleanDatabase();
        Neo4jDbClient.Driver.Dispose();
        await _neo4jTestContainer.DisposeAsync();
    }
}