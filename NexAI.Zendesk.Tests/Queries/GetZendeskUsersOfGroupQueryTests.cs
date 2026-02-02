using FluentAssertions;
using NexAI.Zendesk.Commands;
using NexAI.Zendesk.Queries;
using Xunit;

namespace NexAI.Zendesk.Tests.Queries;

public class GetZendeskUsersOfGroupQueryTests : Neo4jDbBasedTest
{
    [Fact]
    public async Task Handle_WithGroupAndMembers_ReturnsUsersInGroup()
    {
        // arrange
        var upsertGroupCommand = new UpsertZendeskGroupCommand(Neo4jDbClient);
        var upsertUserCommand = new UpsertZendeskUserCommand(Neo4jDbClient);
        var upsertMembersCommand = new UpsertZendeskMembersOfRelationshipCommand(Neo4jDbClient);

        var group1 = new ZendeskGroup(ZendeskGroupId.New(), "group-1", "Test Group One");
        await upsertGroupCommand.Handle(group1);

        var user1 = new ZendeskUser(ZendeskUserId.New(), "user-1", "User One", "user1@example.com");
        await upsertUserCommand.Handle(user1);
        await upsertMembersCommand.Handle(new ZendeskUserGroups(user1.Id, [group1.Id]));

        var user2 = new ZendeskUser(ZendeskUserId.New(), "user-2", "User Two", "user2@example.com");
        await upsertUserCommand.Handle(user2);
        await upsertMembersCommand.Handle(new ZendeskUserGroups(user2.Id, [group1.Id]));

        var group2 = new ZendeskGroup(ZendeskGroupId.New(), "group-2", "Test Group Two");
        await upsertGroupCommand.Handle(group2);

        var user3 = new ZendeskUser(ZendeskUserId.New(), "user-3", "User From Another Group", "user3@example.com");
        await upsertUserCommand.Handle(user3);
        await upsertMembersCommand.Handle(new ZendeskUserGroups(user3.Id, [group2.Id]));

        var query = new GetZendeskUsersOfGroupQuery(Neo4jDbClient);

        // act
        var result = await query.Handle(group1.Id, 10);

        // assert
        result.Should().HaveCount(2);
        result.Should().Contain(user =>
            user.Id == user1.Id && user.ExternalId == "user-1" && user.Name == "User One" && user.Email == "user1@example.com");
        result.Should().Contain(user =>
            user.Id == user2.Id && user.ExternalId == "user-2" && user.Name == "User Two" && user.Email == "user2@example.com");
    }

    [Fact]
    public async Task Handle_WithEmptyGroup_ReturnsEmptyArray()
    {
        // arrange
        var upsertGroupCommand = new UpsertZendeskGroupCommand(Neo4jDbClient);
        var group = new ZendeskGroup(ZendeskGroupId.New(), "group-123", "Empty Group");
        await upsertGroupCommand.Handle(group);

        var query = new GetZendeskUsersOfGroupQuery(Neo4jDbClient);

        // act
        var result = await query.Handle(group.Id, 10);

        // assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithLimit_RespectsLimit()
    {
        // arrange
        var upsertGroupCommand = new UpsertZendeskGroupCommand(Neo4jDbClient);
        var upsertUserCommand = new UpsertZendeskUserCommand(Neo4jDbClient);
        var upsertMembersCommand = new UpsertZendeskMembersOfRelationshipCommand(Neo4jDbClient);

        var group = new ZendeskGroup(ZendeskGroupId.New(), "group-123", "Test Group");
        await upsertGroupCommand.Handle(group);

        for (var i = 0; i < 5; i++)
        {
            var user = new ZendeskUser(ZendeskUserId.New(), $"user-{i}", $"User {i}", $"user{i}@example.com");
            await upsertUserCommand.Handle(user);
            await upsertMembersCommand.Handle(new ZendeskUserGroups(user.Id, [group.Id]));
        }

        var query = new GetZendeskUsersOfGroupQuery(Neo4jDbClient);

        // act
        var result = await query.Handle(group.Id, 2);

        // assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithNonExistentGroup_ReturnsEmptyArray()
    {
        // arrange
        var group = new ZendeskGroup(ZendeskGroupId.New(), "group-123", "Test Group");
        var query = new GetZendeskUsersOfGroupQuery(Neo4jDbClient);

        // act
        var result = await query.Handle(group.Id, 10);

        // assert
        result.Should().BeEmpty();
    }
}