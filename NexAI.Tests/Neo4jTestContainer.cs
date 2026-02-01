// ReSharper disable InconsistentNaming

using Testcontainers.Neo4j;

namespace NexAI.Tests;

public sealed class Neo4jTestContainer : IAsyncDisposable
{
    private readonly Neo4jContainer _neo4jContainer;

    public Dictionary<string, string?> Settings { get; }

    private Neo4jTestContainer(Neo4jContainer neo4jContainer)
    {
        _neo4jContainer = neo4jContainer;
        Settings = new Dictionary<string, string?>
        {
            ["Neo4j:ConnectionString"] = _neo4jContainer.GetConnectionString(),
            ["Neo4j:Username"] = "neo4j",
            ["Neo4j:Password"] = "password"
        };
    }

    public static async Task<Neo4jTestContainer> CreateStartedAsync()
    {
        var neo4JContainer = new Neo4jBuilder("neo4j:latest")
            .WithEnvironment("NEO4J_AUTH", "neo4j/password")
            .Build();
        await neo4JContainer.StartAsync();
        return new Neo4jTestContainer(neo4JContainer);
    }

    public ValueTask DisposeAsync()
    {
        return _neo4jContainer.DisposeAsync();
    }
}
