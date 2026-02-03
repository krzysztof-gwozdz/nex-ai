using MongoDB.Driver;
using NexAI.Tests.MongoDb;
using NexAI.Zendesk.MongoDb;
using NexAI.Zendesk.Queries;
using NexAI.Zendesk.Tests.Builders;

namespace NexAI.Zendesk.Tests.Queries;

[Collection("MongoDb")]
public class FindZendeskTicketsThatContainPhraseQueryTests(MongoDbTestFixture fixture) : MongoDbBasedTest(fixture)
{
    [Fact]
    public async Task Handle_WithMatchingPhraseInTitle_ReturnsMatchingTickets()
    {
        // arrange
        var zendeskTicketMongoDbCollection = new ZendeskTicketMongoDbCollection(MongoDbClient);
        await EnsureFullTextIndexAsync(zendeskTicketMongoDbCollection.Collection);

        var ticket = ZendeskTicketBuilder.Create().WithExternalId("ticket-123").WithTitle("refund request for order").WithDescription("Description").Build();
        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket));

        var query = new FindZendeskTicketsThatContainPhraseQuery(zendeskTicketMongoDbCollection);

        // act
        var result = await query.Handle("refund", 10, CancellationToken.None);

        // assert
        result.Should().HaveCount(1);
        result[0].ZendeskTicket.Id.Should().Be(ticket.Id);
        result[0].ZendeskTicket.Title.Should().Be("refund request for order");
        result[0].Method.Should().Be("full-text");
        result[0].Info.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithMatchingPhraseInDescription_ReturnsMatchingTickets()
    {
        // arrange
        var zendeskTicketMongoDbCollection = new ZendeskTicketMongoDbCollection(MongoDbClient);
        await EnsureFullTextIndexAsync(zendeskTicketMongoDbCollection.Collection);

        var ticket = ZendeskTicketBuilder.Create().WithExternalId("ticket-456").WithTitle("Support").WithDescription("password reset issue").Build();
        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket));

        var query = new FindZendeskTicketsThatContainPhraseQuery(zendeskTicketMongoDbCollection);

        // act
        var result = await query.Handle("password", 10, CancellationToken.None);

        // assert
        result.Should().HaveCount(1);
        result[0].ZendeskTicket.Id.Should().Be(ticket.Id);
        result[0].ZendeskTicket.Description.Should().Be("password reset issue");
    }

    [Fact]
    public async Task Handle_WithMultipleMatchingTickets_ReturnsAllMatching()
    {
        // arrange
        var zendeskTicketMongoDbCollection = new ZendeskTicketMongoDbCollection(MongoDbClient);
        await EnsureFullTextIndexAsync(zendeskTicketMongoDbCollection.Collection);

        var ticket1 = ZendeskTicketBuilder.Create().WithTitle("billing invoice").Build();
        var ticket2 = ZendeskTicketBuilder.Create().WithTitle("invoice question").Build();
        var ticket3 = ZendeskTicketBuilder.Create().WithTitle("invoice and payment").Build();

        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket1));
        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket2));
        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket3));

        var query = new FindZendeskTicketsThatContainPhraseQuery(zendeskTicketMongoDbCollection);

        // act
        var result = await query.Handle("invoice", 10, CancellationToken.None);

        // assert
        result.Should().HaveCount(3);
        result.Should().Contain(searchResult => searchResult.ZendeskTicket.Id == ticket1.Id);
        result.Should().Contain(searchResult => searchResult.ZendeskTicket.Id == ticket2.Id);
        result.Should().Contain(searchResult => searchResult.ZendeskTicket.Id == ticket3.Id);
    }

    [Fact]
    public async Task Handle_WithLimit_ReturnsAtMostLimitResults()
    {
        // arrange
        var zendeskTicketMongoDbCollection = new ZendeskTicketMongoDbCollection(MongoDbClient);
        await EnsureFullTextIndexAsync(zendeskTicketMongoDbCollection.Collection);

        var ticket1 = ZendeskTicketBuilder.Create().WithTitle("billing invoice").Build();
        var ticket2 = ZendeskTicketBuilder.Create().WithTitle("invoice question").Build();
        var ticket3 = ZendeskTicketBuilder.Create().WithTitle("invoice and payment").Build();

        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket1));
        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket2));
        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket3));

        var query = new FindZendeskTicketsThatContainPhraseQuery(zendeskTicketMongoDbCollection);
        const int limit = 2;

        // act
        var result = await query.Handle("invoice", limit, CancellationToken.None);

        // assert
        result.Should().HaveCount(limit);
    }

    [Fact]
    public async Task Handle_WithMultipleMatchingTickets_NormalizesScoresBetweenZeroAndOne()
    {
        // arrange
        var zendeskTicketMongoDbCollection = new ZendeskTicketMongoDbCollection(MongoDbClient);
        await EnsureFullTextIndexAsync(zendeskTicketMongoDbCollection.Collection);

        var ticket1 = ZendeskTicketBuilder.Create().WithTitle("billing invoice").Build();
        var ticket2 = ZendeskTicketBuilder.Create().WithTitle("invoice question").Build();
        var ticket3 = ZendeskTicketBuilder.Create().WithTitle("invoice and payment").Build();

        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket1));
        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket2));
        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket3));

        var query = new FindZendeskTicketsThatContainPhraseQuery(zendeskTicketMongoDbCollection);

        // act
        var result = await query.Handle("invoice", 10, CancellationToken.None);

        // assert
        result.Should().HaveCount(3);
        foreach (var searchResult in result)
            searchResult.Score.Should().BeInRange(0.0, 1.0);
        result.Max(searchResult => searchResult.Score).Should().Be(1.0);
    }

    [Fact]
    public async Task Handle_WithMultipleMatchingTickets_ReturnsResultsSortedByScoreDescending()
    {
        // arrange – more occurrences of the search term typically yield higher text score
        var zendeskTicketMongoDbCollection = new ZendeskTicketMongoDbCollection(MongoDbClient);
        await EnsureFullTextIndexAsync(zendeskTicketMongoDbCollection.Collection);

        var ticket1 = ZendeskTicketBuilder.Create().WithTitle("billing invoice").Build();
        var ticket2 = ZendeskTicketBuilder.Create().WithTitle("invoice question").Build();
        var ticket3 = ZendeskTicketBuilder.Create().WithTitle("invoice and payment").Build();

        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket1));
        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket2));
        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket3));

        var query = new FindZendeskTicketsThatContainPhraseQuery(zendeskTicketMongoDbCollection);

        // act
        var result = await query.Handle("invoice", 10, CancellationToken.None);

        // assert
        result.Should().HaveCount(3);
        result.Should().BeInDescendingOrder(searchResult => searchResult.Score);
    }

    private static async Task EnsureFullTextIndexAsync(IMongoCollection<ZendeskTicketMongoDbDocument> collection)
    {
        var indexKeys = Builders<ZendeskTicketMongoDbDocument>.IndexKeys
            .Text(zendeskTicket => zendeskTicket.Title)
            .Text(zendeskTicket => zendeskTicket.Description)
            .Text(zendeskTicket => zendeskTicket.Messages.Select(message => message.Content));
        var indexModel = new CreateIndexModel<ZendeskTicketMongoDbDocument>(indexKeys);
        await collection.Indexes.CreateOneAsync(indexModel);
    }
}
