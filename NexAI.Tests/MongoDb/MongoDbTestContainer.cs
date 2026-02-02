// ReSharper disable InconsistentNaming

using System.Globalization;
using Testcontainers.MongoDb;

namespace NexAI.Tests.MongoDb;

public sealed class MongoDbTestContainer : IAsyncDisposable
{
    private readonly MongoDbContainer _mongoDbContainer;

    public Dictionary<string, string?> Settings { get; }

    private MongoDbTestContainer(MongoDbContainer mongoDbContainer)
    {
        _mongoDbContainer = mongoDbContainer;

        var port = _mongoDbContainer.GetMappedPublicPort(MongoDbBuilder.MongoDbPort);

        Settings = new Dictionary<string, string?>
        {
            ["MongoDb:Host"] = _mongoDbContainer.Hostname,
            ["MongoDb:Port"] = port.ToString(CultureInfo.InvariantCulture),
            ["MongoDb:Database"] = "test",
            ["MongoDb:Username"] = "mongo",
            ["MongoDb:Password"] = "password"
        };
    }

    public static async Task<MongoDbTestContainer> CreateStartedAsync()
    {
        var mongoDbContainer = new MongoDbBuilder("mongo:latest")
            .WithUsername("mongo")
            .WithPassword("password")
            .Build();

        await mongoDbContainer.StartAsync();

        return new MongoDbTestContainer(mongoDbContainer);
    }

    public ValueTask DisposeAsync()
    {
        return _mongoDbContainer.DisposeAsync();
    }
}

