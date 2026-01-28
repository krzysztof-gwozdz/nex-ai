// ReSharper disable InconsistentNaming

using Microsoft.Extensions.Configuration;
using Neo4j.Driver;
using NexAI.Config;
using NexAI.Neo4j;
using Testcontainers.Neo4j;
using Xunit;

namespace NexAI.Zendesk.Tests;

public class Neo4jTestBase : IAsyncLifetime
{
    private Neo4jContainer _neo4jContainer = null!;
    protected Neo4jDbClient Neo4jDbClient { get; private set; } = null!;
    protected IDriver Driver { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _neo4jContainer = new Neo4jBuilder("neo4j:latest")
            .WithEnvironment("NEO4J_AUTH", "neo4j/password")
            .Build();

        await _neo4jContainer.StartAsync();

        var connectionString = _neo4jContainer.GetConnectionString();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Neo4j:ConnectionString"] = connectionString,
                ["Neo4j:Username"] = "neo4j",
                ["Neo4j:Password"] = "password"
            })
            .Build();

        var options = new Options(configuration);
        Neo4jDbClient = new(options);
        Driver = Neo4jDbClient.Driver;
    }

    public async Task DisposeAsync()
    {
        Driver.Dispose();
        await _neo4jContainer.DisposeAsync();
    }

    protected async Task CleanDatabaseAsync()
    {
        await using var session = Driver.AsyncSession(sessionConfigBuilder => sessionConfigBuilder.WithDatabase("neo4j"));
        await session.RunAsync("MATCH (n) DETACH DELETE n");
    }

    protected async Task<IRecord?> GetNode(string label, string property, object value)
    {
        await using var session = Driver.AsyncSession(sessionConfigBuilder => sessionConfigBuilder.WithDatabase("neo4j"));
        var result = await session.RunAsync(
            $"MATCH (n:{label} {{{property}: $value}}) RETURN n",
            new Dictionary<string, object> { { "value", value } });
        await result.FetchAsync();
        return result.Current;
    }

    protected async Task<IRecord> GetRelationship(string fromLabel, string fromProperty, object fromValue, string relationshipType, string toLabel, string toProperty, object toValue)
    {
        await using var session = Driver.AsyncSession(sessionConfigBuilder => sessionConfigBuilder.WithDatabase("neo4j"));
        var result = await session.RunAsync(
            $"MATCH (a:{fromLabel} {{{fromProperty}: $fromValue}})-[r:{relationshipType}]->(b:{toLabel} {{{toProperty}: $toValue}}) RETURN r",
            new Dictionary<string, object>
            {
                { "fromValue", fromValue },
                { "toValue", toValue }
            });
        await result.FetchAsync();
        return result.Current;
    }
}