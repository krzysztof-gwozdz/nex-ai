// ReSharper disable InconsistentNaming

using Microsoft.Extensions.Configuration;
using NexAI.Config;
using NexAI.Neo4j;
using Testcontainers.Neo4j;
using Xunit;

namespace NexAI.Tests.Neo4j;

public sealed class Neo4jTestFixture : IAsyncLifetime
{
    private Neo4jContainer _neo4jContainer = null!;

    public Neo4jDbClient Neo4jDbClient { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _neo4jContainer = new Neo4jBuilder("neo4j:latest")
            .WithEnvironment("NEO4J_AUTH", "neo4j/password")
            .Build();
        await _neo4jContainer.StartAsync();
        var settings = new Dictionary<string, string?>()
            .Concat(new Dictionary<string, string?>
            {
                ["Neo4j:ConnectionString"] = _neo4jContainer.GetConnectionString(),
                ["Neo4j:Username"] = "neo4j",
                ["Neo4j:Password"] = "password"
            });
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
        var options = new Options(configuration);
        Neo4jDbClient = new(options);
    }

    public async Task DisposeAsync()
    {
        Neo4jDbClient.Driver.Dispose();
        await _neo4jContainer.DisposeAsync();
    }
}
