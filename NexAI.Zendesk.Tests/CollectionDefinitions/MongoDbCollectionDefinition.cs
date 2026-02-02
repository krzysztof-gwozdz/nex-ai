// ReSharper disable InconsistentNaming

using NexAI.Tests.MongoDb;
using Xunit;

namespace NexAI.Zendesk.Tests.CollectionDefinitions;

[CollectionDefinition("MongoDb")]
public sealed class MongoDbCollectionDefinition : ICollectionFixture<MongoDbTestFixture>;
