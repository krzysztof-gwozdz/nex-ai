// ReSharper disable InconsistentNaming

using NexAI.Neo4j;
using Xunit;

namespace NexAI.Tests.Neo4j;

public class Neo4jDbBasedTest(Neo4jTestFixture fixture) : IAsyncLifetime
{
    protected Neo4jDbClient Neo4jDbClient => fixture.Neo4jDbClient;

    public async Task InitializeAsync() => await Neo4jDbClient.CleanDatabase();

    public Task DisposeAsync() => Task.CompletedTask;
}
