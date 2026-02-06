using MongoDB.Driver;
using NexAI.LLMs.Common;
using NexAI.LLMs.Fake;
using NexAI.LLMs.MongoDb;
using NexAI.Tests.MongoDb;

namespace NexAI.LLMs.Tests.MongoDb;

[Collection("MongoDb")]
public class MongoDbChatDecoratorTests(MongoDbTestFixture fixture) : MongoDbBasedTest(fixture)
{
    private readonly CancellationTokenSource _cancellationTokenSource = new(TimeSpan.FromSeconds(60));

    [Fact]
    public async Task Ask_ReturnResponse()
    {
        // arrange
        var chat = new FakeChat();
        var collection = new ConversationMongoDbCollection(MongoDbClient);
        var sut = new MongoDbChatDecorator(chat, collection);

        // act
        var conversationId = ConversationId.New();
        var answer = await sut.Ask(conversationId, "JUST SAY: TEST, nothing else.", "Hi", _cancellationTokenSource.Token);

        // assert
        answer.Should().Be("TEST");
    }

    [Fact]
    public async Task Ask_UpsertsConversation()
    {
        // arrange
        var chat = new FakeChat();
        var collection = new ConversationMongoDbCollection(MongoDbClient);
        var sut = new MongoDbChatDecorator(chat, collection);
        var conversationId = ConversationId.New();

        // act
        await sut.Ask(conversationId, "JUST SAY: TEST, nothing else.", "Hi", _cancellationTokenSource.Token);

        // assert
        var filter = Builders<ConversationMongoDbDocument>.Filter.Eq(d => d.Id, conversationId);
        var document = await collection.Collection.Find(filter).FirstOrDefaultAsync(_cancellationTokenSource.Token);
        document.Should().NotBeNull();
        document.Messages.Should().HaveCount(3); // system, user, assistant
        document.Messages[0].Role.Should().Be("system");
        document.Messages[0].Content.Should().Be("JUST SAY: TEST, nothing else.");
        document.Messages[1].Role.Should().Be("user");
        document.Messages[1].Content.Should().Be("Hi");
        document.Messages[2].Role.Should().Be("assistant");
        document.Messages[2].Content.Should().Be("TEST");
    }

    [Fact]
    public async Task Ask_WithSpecificObject_ReturnResponse()
    {
        // arrange
        var chat = new FakeChat();
        var collection = new ConversationMongoDbCollection(MongoDbClient);
        var sut = new MongoDbChatDecorator(chat, collection);

        // act
        var conversationId = ConversationId.New();
        var answer = await sut.Ask<string>(conversationId, "JUST SAY: TEST, nothing else.", "Hi", _cancellationTokenSource.Token);

        // assert
        answer.Should().Be("TEST");
    }

    [Fact]
    public async Task Ask_WithSpecificObject_UpsertsConversation()
    {
        // arrange
        var chat = new FakeChat();
        var collection = new ConversationMongoDbCollection(MongoDbClient);
        var sut = new MongoDbChatDecorator(chat, collection);
        var conversationId = ConversationId.New();

        // act
        await sut.Ask<string>(conversationId, "JUST SAY: TEST, nothing else.", "Hi", _cancellationTokenSource.Token);

        // assert
        var filter = Builders<ConversationMongoDbDocument>.Filter.Eq(d => d.Id, conversationId);
        var document = await collection.Collection.Find(filter).FirstOrDefaultAsync(_cancellationTokenSource.Token);
        document.Should().NotBeNull();
        document.Messages.Should().HaveCount(3); // system, user, assistant
        document.Messages[0].Role.Should().Be("system");
        document.Messages[0].Content.Should().Be("JUST SAY: TEST, nothing else.");
        document.Messages[1].Role.Should().Be("user");
        document.Messages[1].Content.Should().Be("Hi");
        document.Messages[2].Role.Should().Be("assistant");
        document.Messages[2].Content.Should().Be("TEST");
    }

    [Fact]
    public async Task Ask_WhenDocumentWithSameConversationIdExists_ReplacesDocument()
    {
        // arrange
        var chat = new FakeChat();
        var collection = new ConversationMongoDbCollection(MongoDbClient);
        var sut = new MongoDbChatDecorator(chat, collection);
        var conversationId = ConversationId.New();
        var existingDocument = new ConversationMongoDbDocument(
            conversationId,
            [ConversationMongoDbDocument.MessageMongoDbDocument.Create(new ChatMessage("user", "old")), ConversationMongoDbDocument.MessageMongoDbDocument.Create(new ChatMessage("assistant", "old reply"))]);
        await collection.Collection.InsertOneAsync(existingDocument, cancellationToken: _cancellationTokenSource.Token);

        // act
        await sut.Ask(conversationId, "JUST SAY: TEST, nothing else.", "Hi", _cancellationTokenSource.Token);

        // assert
        var filter = Builders<ConversationMongoDbDocument>.Filter.Eq(d => d.Id, conversationId);
        var documents = await collection.Collection.Find(filter).ToListAsync(_cancellationTokenSource.Token);
        documents.Should().HaveCount(1);
        var document = documents[0];
        document.Messages.Should().HaveCount(3);
        document.Messages[0].Content.Should().Be("JUST SAY: TEST, nothing else.");
        document.Messages[1].Content.Should().Be("Hi");
        document.Messages[2].Content.Should().Be("TEST");
    }

    [Fact]
    public async Task AskStream_ReturnResponse()
    {
        // arrange
        var chat = new FakeChat();
        var collection = new ConversationMongoDbCollection(MongoDbClient);
        var sut = new MongoDbChatDecorator(chat, collection);

        // act
        var answer = string.Empty;
        var response = sut.AskStream(ConversationId.New(), "JUST SAY: TEST, nothing else.", "Hi", _cancellationTokenSource.Token);
        await foreach (var message in response)
        {
            answer += message;
        }

        // assert
        answer.Should().Be("TEST");
    }

    [Fact]
    public async Task AskStream_UpsertsConversation()
    {
        // arrange
        var chat = new FakeChat();
        var collection = new ConversationMongoDbCollection(MongoDbClient);
        var sut = new MongoDbChatDecorator(chat, collection);
        var conversationId = ConversationId.New();

        // act
        await foreach (var _ in sut.AskStream(conversationId, "JUST SAY: TEST, nothing else.", "Hi", _cancellationTokenSource.Token))
        {
        }

        // assert
        var filter = Builders<ConversationMongoDbDocument>.Filter.Eq(d => d.Id, conversationId);
        var document = await collection.Collection.Find(filter).FirstOrDefaultAsync(_cancellationTokenSource.Token);
        document.Should().NotBeNull();
        document.Messages.Should().HaveCount(3);
        document.Messages[2].Role.Should().Be("assistant");
        document.Messages[2].Content.Should().Be("TEST");
    }

    [Fact]
    public async Task AskStream_WhenDocumentWithSameConversationIdExists_ReplacesDocument()
    {
        // arrange
        var chat = new FakeChat();
        var collection = new ConversationMongoDbCollection(MongoDbClient);
        var sut = new MongoDbChatDecorator(chat, collection);
        var conversationId = ConversationId.New();
        var existingDocument = new ConversationMongoDbDocument(
            conversationId,
            [ConversationMongoDbDocument.MessageMongoDbDocument.Create(new ChatMessage("user", "old")), ConversationMongoDbDocument.MessageMongoDbDocument.Create(new ChatMessage("assistant", "old reply"))]);
        await collection.Collection.InsertOneAsync(existingDocument, cancellationToken: _cancellationTokenSource.Token);

        // act
        await foreach (var _ in sut.AskStream(conversationId, "JUST SAY: TEST, nothing else.", "Hi", _cancellationTokenSource.Token))
        {
        }

        // assert
        var filter = Builders<ConversationMongoDbDocument>.Filter.Eq(d => d.Id, conversationId);
        var documents = await collection.Collection.Find(filter).ToListAsync(_cancellationTokenSource.Token);
        documents.Should().HaveCount(1);
        var document = documents[0];
        document.Messages.Should().HaveCount(3);
        document.Messages[2].Content.Should().Be("TEST");
    }

    [Fact]
    public async Task GetNextResponse_ReturnResponse()
    {
        // arrange
        var chat = new FakeChat();
        var collection = new ConversationMongoDbCollection(MongoDbClient);
        var sut = new MongoDbChatDecorator(chat, collection);
        var messages = new[] { new ChatMessage("system", "JUST SAY: TEST, nothing else."), new ChatMessage("user", "Hi") };

        // act
        var answer = await sut.GetNextResponse(ConversationId.New(), messages, _cancellationTokenSource.Token);

        // assert
        answer.Should().Be("TEST");
    }

    [Fact]
    public async Task GetNextResponse_UpsertsConversation()
    {
        // arrange
        var chat = new FakeChat();
        var collection = new ConversationMongoDbCollection(MongoDbClient);
        var sut = new MongoDbChatDecorator(chat, collection);
        var conversationId = ConversationId.New();
        var messages = new[] { new ChatMessage("system", "JUST SAY: TEST, nothing else."), new ChatMessage("user", "Hi") };

        // act
        await sut.GetNextResponse(conversationId, messages, _cancellationTokenSource.Token);

        // assert
        var filter = Builders<ConversationMongoDbDocument>.Filter.Eq(d => d.Id, conversationId);
        var document = await collection.Collection.Find(filter).FirstOrDefaultAsync(_cancellationTokenSource.Token);
        document.Should().NotBeNull();
        document.Messages.Should().HaveCount(3);
        document.Messages[2].Content.Should().Be("TEST");
    }

    [Fact]
    public async Task GetNextResponse_WhenDocumentWithSameConversationIdExists_ReplacesDocument()
    {
        // arrange
        var chat = new FakeChat();
        var collection = new ConversationMongoDbCollection(MongoDbClient);
        var sut = new MongoDbChatDecorator(chat, collection);
        var conversationId = ConversationId.New();
        var messages = new[] { new ChatMessage("system", "JUST SAY: TEST, nothing else."), new ChatMessage("user", "Hi") };
        var existingDocument = new ConversationMongoDbDocument(
            conversationId,
            [ConversationMongoDbDocument.MessageMongoDbDocument.Create(new ChatMessage("user", "old")), ConversationMongoDbDocument.MessageMongoDbDocument.Create(new ChatMessage("assistant", "old reply"))]);
        await collection.Collection.InsertOneAsync(existingDocument, cancellationToken: _cancellationTokenSource.Token);

        // act
        await sut.GetNextResponse(conversationId, messages, _cancellationTokenSource.Token);

        // assert
        var filter = Builders<ConversationMongoDbDocument>.Filter.Eq(d => d.Id, conversationId);
        var documents = await collection.Collection.Find(filter).ToListAsync(_cancellationTokenSource.Token);
        documents.Should().HaveCount(1);
        var document = documents[0];
        document.Messages.Should().HaveCount(3);
        document.Messages[2].Content.Should().Be("TEST");
    }

    [Fact]
    public async Task StreamNextResponse_ReturnResponse()
    {
        // arrange
        var chat = new FakeChat();
        var collection = new ConversationMongoDbCollection(MongoDbClient);
        var sut = new MongoDbChatDecorator(chat, collection);
        var messages = new[] { new ChatMessage("system", "JUST SAY: TEST, nothing else."), new ChatMessage("user", "Hi") };

        // act
        var answer = string.Empty;
        var response = sut.StreamNextResponse(ConversationId.New(), messages, _cancellationTokenSource.Token);
        await foreach (var message in response)
        {
            answer += message;
        }

        // assert
        answer.Should().Be("TEST");
    }

    [Fact]
    public async Task StreamNextResponse_UpsertsConversation()
    {
        // arrange
        var chat = new FakeChat();
        var collection = new ConversationMongoDbCollection(MongoDbClient);
        var sut = new MongoDbChatDecorator(chat, collection);
        var conversationId = ConversationId.New();
        var messages = new[] { new ChatMessage("system", "JUST SAY: TEST, nothing else."), new ChatMessage("user", "Hi") };

        // act
        await foreach (var _ in sut.StreamNextResponse(conversationId, messages, _cancellationTokenSource.Token))
        {
        }

        // assert
        var filter = Builders<ConversationMongoDbDocument>.Filter.Eq(d => d.Id, conversationId);
        var document = await collection.Collection.Find(filter).FirstOrDefaultAsync(_cancellationTokenSource.Token);
        document.Should().NotBeNull();
        document.Messages.Should().HaveCount(3);
        document.Messages[2].Content.Should().Be("TEST");
    }

    [Fact]
    public async Task StreamNextResponse_WhenDocumentWithSameConversationIdExists_ReplacesDocument()
    {
        // arrange
        var chat = new FakeChat();
        var collection = new ConversationMongoDbCollection(MongoDbClient);
        var sut = new MongoDbChatDecorator(chat, collection);
        var conversationId = ConversationId.New();
        var messages = new[] { new ChatMessage("system", "JUST SAY: TEST, nothing else."), new ChatMessage("user", "Hi") };
        var existingDocument = new ConversationMongoDbDocument(
            conversationId,
            [ConversationMongoDbDocument.MessageMongoDbDocument.Create(new ChatMessage("user", "old")), ConversationMongoDbDocument.MessageMongoDbDocument.Create(new ChatMessage("assistant", "old reply"))]);
        await collection.Collection.InsertOneAsync(existingDocument, cancellationToken: _cancellationTokenSource.Token);

        // act
        await foreach (var _ in sut.StreamNextResponse(conversationId, messages, _cancellationTokenSource.Token))
        {
        }

        // assert
        var filter = Builders<ConversationMongoDbDocument>.Filter.Eq(d => d.Id, conversationId);
        var documents = await collection.Collection.Find(filter).ToListAsync(_cancellationTokenSource.Token);
        documents.Should().HaveCount(1);
        var document = documents[0];
        document.Messages.Should().HaveCount(3);
        document.Messages[2].Content.Should().Be("TEST");
    }
}
