using NexAI.Tests.MongoDb;

namespace NexAI.LLMs.Tests.MongoDb;

[CollectionDefinition("MongoDb")]
public class MongoDbCollectionDefinition : ICollectionFixture<MongoDbTestFixture>;
