// ReSharper disable InconsistentNaming

using NexAI.Tests.Neo4j;
using Xunit;

namespace NexAI.Zendesk.Tests.CollectionDefinitions;

[CollectionDefinition("Neo4j")]
public sealed class Neo4jCollectionDefinition : ICollectionFixture<Neo4jTestFixture>;
