// ReSharper disable InconsistentNaming

using MongoDB.Driver;
using NexAI.MongoDb;
using Xunit;

namespace NexAI.Tests.MongoDb;

public class MongoDbBasedTest(MongoDbTestFixture fixture) : IAsyncLifetime
{
    protected MongoDbClient MongoDbClient => fixture.MongoDbClient;

    public async Task InitializeAsync()
    {
        var database = MongoDbClient.Database;
        using var cursor = await database.ListCollectionNamesAsync();
        var collections = await cursor.ToListAsync();
        foreach (var collection in collections)
        {
            await database.DropCollectionAsync(collection);
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;
}