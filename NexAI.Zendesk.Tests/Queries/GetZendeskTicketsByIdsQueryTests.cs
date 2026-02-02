using FluentAssertions;
using NexAI.Tests.MongoDb;
using NexAI.Zendesk.MongoDb;
using NexAI.Zendesk.Queries;
using NexAI.Zendesk.Tests.Builders;
using Xunit;

namespace NexAI.Zendesk.Tests.Queries;

[Collection("MongoDb")]
public class GetZendeskTicketsByIdsQueryTests(MongoDbTestFixture fixture) : MongoDbBasedTest(fixture)
{
    [Fact]
    public async Task Handle_WithExistingIds_ReturnsMatchingTickets()
    {
        // arrange
        var zendeskTicketMongoDbCollection = new ZendeskTicketMongoDbCollection(MongoDbClient);
        var ticket = ZendeskTicketBuilder.Create().WithTitle("Support Request").Build();
        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket));

        var query = new GetZendeskTicketsByIdsQuery(zendeskTicketMongoDbCollection);

        // act
        var result = await query.Handle([ticket.Id], CancellationToken.None);

        // assert
        result.Should().HaveCount(1);
        result[0].Id.Should().Be(ticket.Id);
    }

    [Fact]
    public async Task Handle_WithMultipleIds_ReturnsTicketsWithMatchingIds()
    {
        // arrange
        var zendeskTicketMongoDbCollection = new ZendeskTicketMongoDbCollection(MongoDbClient);

        var ticket1 = ZendeskTicketBuilder.Create().WithTitle("First Ticket").Build();
        var ticket2 = ZendeskTicketBuilder.Create().WithTitle("Second Ticket").Build();
        var ticket3 = ZendeskTicketBuilder.Create().WithTitle("Third Ticket").Build();

        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket1));
        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket2));
        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket3));

        var query = new GetZendeskTicketsByIdsQuery(zendeskTicketMongoDbCollection);

        // act
        var result = await query.Handle([ticket1.Id, ticket3.Id], CancellationToken.None);

        // assert
        result.Should().HaveCount(2);
        result.Should().Contain(zendeskTicket => zendeskTicket.Id == ticket1.Id);
        result.Should().Contain(zendeskTicket => zendeskTicket.Id == ticket3.Id);
    }

    [Fact]
    public async Task Handle_WithNonExistentIds_ReturnsEmptyArray()
    {
        // arrange
        var zendeskTicketMongoDbCollection = new ZendeskTicketMongoDbCollection(MongoDbClient);
        var query = new GetZendeskTicketsByIdsQuery(zendeskTicketMongoDbCollection);
        var nonExistentIds = new[] { Guid.CreateVersion7(), Guid.CreateVersion7() };

        // act
        var result = await query.Handle(nonExistentIds, CancellationToken.None);

        // assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithEmptyIds_ReturnsEmptyArray()
    {
        // arrange
        var zendeskTicketMongoDbCollection = new ZendeskTicketMongoDbCollection(MongoDbClient);
        var query = new GetZendeskTicketsByIdsQuery(zendeskTicketMongoDbCollection);

        // act
        var result = await query.Handle([], CancellationToken.None);

        // assert
        result.Should().BeEmpty();
    }
}
