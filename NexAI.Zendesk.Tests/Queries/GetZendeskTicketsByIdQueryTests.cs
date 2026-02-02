using FluentAssertions;
using NexAI.Tests.MongoDb;
using NexAI.Zendesk.MongoDb;
using NexAI.Zendesk.Queries;
using NexAI.Zendesk.Tests.Builders;
using Xunit;

namespace NexAI.Zendesk.Tests.Queries;

[Collection("MongoDb")]
public class GetZendeskTicketsByIdQueryTests(MongoDbTestFixture fixture) : MongoDbBasedTest(fixture)
{
    [Fact]
    public async Task Handle_WithExistingId_ReturnsMatchingTicket()
    {
        // arrange
        var zendeskTicketMongoDbCollection = new ZendeskTicketMongoDbCollection(MongoDbClient);
        var ticket = ZendeskTicketBuilder.Create().WithTitle("Support Request").Build();
        var document = ZendeskTicketMongoDbDocument.Create(ticket);
        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(document);

        var query = new GetZendeskTicketsByIdQuery(zendeskTicketMongoDbCollection);

        // act
        var result = await query.Handle(ticket.Id, CancellationToken.None);

        // assert
        result.Should().NotBeNull();
        result.Id.Should().Be(ticket.Id);
    }

    [Fact]
    public async Task Handle_WithMultipleTickets_ReturnsTicketWithMatchingId()
    {
        // arrange
        var zendeskTicketMongoDbCollection = new ZendeskTicketMongoDbCollection(MongoDbClient);

        var ticket1 = ZendeskTicketBuilder.Create().WithTitle("First Ticket").Build();
        var ticket2 = ZendeskTicketBuilder.Create().WithTitle("Second Ticket").Build();
        var ticket3 = ZendeskTicketBuilder.Create().WithTitle("Third Ticket").Build();

        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket1));
        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket2));
        await zendeskTicketMongoDbCollection.Collection.InsertOneAsync(ZendeskTicketMongoDbDocument.Create(ticket3));

        var query = new GetZendeskTicketsByIdQuery(zendeskTicketMongoDbCollection);

        // act
        var result = await query.Handle(ticket2.Id, CancellationToken.None);

        // assert
        result.Should().NotBeNull();
        result.Id.Should().Be(ticket2.Id);
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ReturnsNull()
    {
        // arrange
        var zendeskTicketMongoDbCollection = new ZendeskTicketMongoDbCollection(MongoDbClient);
        var query = new GetZendeskTicketsByIdQuery(zendeskTicketMongoDbCollection);
        var nonExistentId = Guid.CreateVersion7();

        // act
        var result = await query.Handle(nonExistentId, CancellationToken.None);

        // assert
        result.Should().BeNull();
    }
}
