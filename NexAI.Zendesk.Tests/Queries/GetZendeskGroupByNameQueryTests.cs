using FluentAssertions;
using NexAI.Zendesk.Commands;
using NexAI.Zendesk.Queries;
using Xunit;

namespace NexAI.Zendesk.Tests.Queries;

public class GetZendeskGroupByNameQueryTests : Neo4jDbBasedTest
{
    [Fact]
    public async Task Handle_WithExistingGroupName_ReturnsMatchingGroup()
    {
        // arrange
        var upsertGroupCommand = new UpsertZendeskGroupCommand(Neo4jDbClient);
        var group = new ZendeskGroup(ZendeskGroupId.New(), "group-123", "Support Team");
        await upsertGroupCommand.Handle(group);

        var query = new GetZendeskGroupByNameQuery(Neo4jDbClient);

        // act
        var result = await query.Handle("Support Team");

        // assert
        result.Should().NotBeNull();
        result.Id.Should().Be(group.Id);
        result.ExternalId.Should().Be("group-123");
        result.Name.Should().Be("Support Team");
    }

    [Fact]
    public async Task Handle_WithMultipleGroups_ReturnsGroupWithMatchingName()
    {
        // arrange
        var upsertGroupCommand = new UpsertZendeskGroupCommand(Neo4jDbClient);

        var group1 = new ZendeskGroup(ZendeskGroupId.New(), "group-1", "First Group");
        var group2 = new ZendeskGroup(ZendeskGroupId.New(), "group-2", "Second Group");
        var group3 = new ZendeskGroup(ZendeskGroupId.New(), "group-3", "Third Group");

        await upsertGroupCommand.Handle(group1);
        await upsertGroupCommand.Handle(group2);
        await upsertGroupCommand.Handle(group3);

        var query = new GetZendeskGroupByNameQuery(Neo4jDbClient);

        // act
        var result = await query.Handle("Second Group");

        // assert
        result.Should().NotBeNull();
        result.Id.Should().Be(group2.Id);
        result.Name.Should().Be("Second Group");
    }

    [Fact]
    public async Task Handle_WithNonExistentGroupName_Throws()
    {
        // arrange
        var query = new GetZendeskGroupByNameQuery(Neo4jDbClient);

        // act
        var act = () => query.Handle("Non Existent Group");

        // assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
