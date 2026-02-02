using FluentAssertions;
using NexAI.Tests.MongoDb;
using NexAI.Zendesk.MongoDb;
using NexAI.Zendesk.Queries;
using NexAI.Zendesk.Tests.Builders;
using Xunit;

namespace NexAI.Zendesk.Tests.Queries;

[Collection("MongoDb")]
public class GetZendeskTicketsByExternalIdsQueryTests(MongoDbTestFixture fixture) : MongoDbBasedTest(fixture)
{
    [Fact]
    public async Task Handle_WithExistingExternalIds_ReturnsMatchingTickets()
    {
        // arrange
        var zendeskTicketMongoDbCollection = new ZendeskTicketMongoDbCollection(MongoDbClient);
        var ticket = ZendeskTicketBuilder.Create().WithExternalId("ticket-1").WithTitle("Support Request").Build();
        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket));

        var query = new GetZendeskTicketsByExternalIdsQuery(zendeskTicketMongoDbCollection);

        // act
        var result = await query.Handle([ticket.ExternalId], CancellationToken.None);

        // assert
        result.Should().HaveCount(1);
        result[0].Id.Should().Be(ticket.Id);
        result[0].ExternalId.Should().Be(ticket.ExternalId);
    }

    [Fact]
    public async Task Handle_WithMultipleExternalIds_ReturnsTicketsWithMatchingExternalIds()
    {
        // arrange
        var zendeskTicketMongoDbCollection = new ZendeskTicketMongoDbCollection(MongoDbClient);

        var ticket1 = ZendeskTicketBuilder.Create().WithExternalId("ticket-1").WithTitle("First Ticket").Build();
        var ticket2 = ZendeskTicketBuilder.Create().WithExternalId("ticket-2").WithTitle("Second Ticket").Build();
        var ticket3 = ZendeskTicketBuilder.Create().WithExternalId("ticket-3").WithTitle("Third Ticket").Build();

        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket1));
        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket2));
        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket3));

        var query = new GetZendeskTicketsByExternalIdsQuery(zendeskTicketMongoDbCollection);

        // act
        var result = await query.Handle([ticket1.ExternalId, ticket3.ExternalId], CancellationToken.None);

        // assert
        result.Should().HaveCount(2);
        result.Should().Contain(zendeskTicket => zendeskTicket.Id == ticket1.Id);
        result.Should().Contain(zendeskTicket => zendeskTicket.Id == ticket3.Id);
    }

    [Fact]
    public async Task Handle_WithNonExistentExternalIds_ReturnsEmptyArray()
    {
        // arrange
        var zendeskTicketMongoDbCollection = new ZendeskTicketMongoDbCollection(MongoDbClient);
        var query = new GetZendeskTicketsByExternalIdsQuery(zendeskTicketMongoDbCollection);
        var nonExistentExternalIds = new[] { "non-existent-1", "non-existent-2" };

        // act
        var result = await query.Handle(nonExistentExternalIds, CancellationToken.None);

        // assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithEmptyExternalIds_ReturnsEmptyArray()
    {
        // arrange
        var zendeskTicketMongoDbCollection = new ZendeskTicketMongoDbCollection(MongoDbClient);
        var query = new GetZendeskTicketsByExternalIdsQuery(zendeskTicketMongoDbCollection);

        // act
        var result = await query.Handle([], CancellationToken.None);

        // assert
        result.Should().BeEmpty();
    }
}
