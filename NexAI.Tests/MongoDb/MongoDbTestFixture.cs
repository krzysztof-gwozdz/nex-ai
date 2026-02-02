// ReSharper disable InconsistentNaming

using System.Globalization;
using Microsoft.Extensions.Configuration;
using NexAI.Config;
using NexAI.MongoDb;
using Testcontainers.MongoDb;
using Xunit;

namespace NexAI.Tests.MongoDb;

public sealed class MongoDbTestFixture : IAsyncLifetime
{
    private MongoDbContainer _mongoDbContainer = null!;

    public MongoDbClient MongoDbClient { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _mongoDbContainer = new MongoDbBuilder("mongo:latest")
            .WithUsername("mongo")
            .WithPassword("password")
            .Build();
        await _mongoDbContainer.StartAsync();
        var port = _mongoDbContainer.GetMappedPublicPort(MongoDbBuilder.MongoDbPort);
        var settings = new Dictionary<string, string?>()
            .Concat(new Dictionary<string, string?>
            {
                ["MongoDb:Host"] = _mongoDbContainer.Hostname,
                ["MongoDb:Port"] = port.ToString(CultureInfo.InvariantCulture),
                ["MongoDb:Database"] = "test",
                ["MongoDb:Username"] = "mongo",
                ["MongoDb:Password"] = "password"
            });
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
        var options = new Options(configuration);
        MongoDbClient = new(options);
    }

    public async Task DisposeAsync() => await _mongoDbContainer.DisposeAsync();
}
