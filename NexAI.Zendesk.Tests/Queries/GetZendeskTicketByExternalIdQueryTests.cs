using FluentAssertions;
using NexAI.Tests.MongoDb;
using NexAI.Zendesk.MongoDb;
using NexAI.Zendesk.Queries;
using NexAI.Zendesk.Tests.Builders;
using Xunit;

namespace NexAI.Zendesk.Tests.Queries;

[Collection("MongoDb")]
public class GetZendeskTicketByExternalIdQueryTests(MongoDbTestFixture fixture) : MongoDbBasedTest(fixture)
{
    [Fact]
    public async Task Handle_WithExistingExternalId_ReturnsMatchingTicket()
    {
        // arrange
        var zendeskTicketMongoDbCollection = new ZendeskTicketMongoDbCollection(MongoDbClient);
        var ticket = ZendeskTicketBuilder.Create().WithExternalId("ticket-1").Build();
        var document = ZendeskTicketMongoDbDocument.Create(ticket);
        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(document);

        var query = new GetZendeskTicketByExternalIdQuery(zendeskTicketMongoDbCollection);

        // act
        var result = await query.Handle(ticket.ExternalId, CancellationToken.None);

        // assert
        result.Should().NotBeNull();
        result.Id.Should().Be(ticket.Id);
        result.ExternalId.Should().Be(ticket.ExternalId);
    }

    [Fact]
    public async Task Handle_WithMultipleTickets_ReturnsTicketWithMatchingExternalId()
    {
        // arrange
        var zendeskTicketMongoDbCollection = new ZendeskTicketMongoDbCollection(MongoDbClient);

        var ticket1 = ZendeskTicketBuilder.Create().WithExternalId("ticket-1").WithTitle("First Ticket").Build();
        var ticket2 = ZendeskTicketBuilder.Create().WithExternalId("ticket-2").WithTitle("Second Ticket").Build();
        var ticket3 = ZendeskTicketBuilder.Create().WithExternalId("ticket-3").WithTitle("Third Ticket").Build();

        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket1));
        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket2));
        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket3));

        var query = new GetZendeskTicketByExternalIdQuery(zendeskTicketMongoDbCollection);

        // act
        var result = await query.Handle(ticket2.ExternalId, CancellationToken.None);

        // assert
        result.Should().NotBeNull();
        result.Id.Should().Be(ticket2.Id);
        result.ExternalId.Should().Be(ticket2.ExternalId);
    }

    [Fact]
    public async Task Handle_WithNonExistentExternalId_ReturnsNull()
    {
        // arrange
        var zendeskTicketMongoDbCollection = new ZendeskTicketMongoDbCollection(MongoDbClient);
        var query = new GetZendeskTicketByExternalIdQuery(zendeskTicketMongoDbCollection);
        const string nonExistentExternalId = "non-existent-ticket";

        // act
        var result = await query.Handle(nonExistentExternalId, CancellationToken.None);

        // assert
        result.Should().BeNull();
    }
}
