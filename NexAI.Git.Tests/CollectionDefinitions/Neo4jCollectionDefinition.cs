// ReSharper disable InconsistentNaming

using NexAI.Tests.Neo4j;

namespace NexAI.Git.Tests.CollectionDefinitions;

[CollectionDefinition("Neo4j")]
public sealed class Neo4jCollectionDefinition : ICollectionFixture<Neo4jTestFixture>;
