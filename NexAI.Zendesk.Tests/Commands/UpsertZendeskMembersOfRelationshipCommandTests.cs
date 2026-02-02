using FluentAssertions;
using NexAI.Tests;
using NexAI.Zendesk.Commands;
using Xunit;

namespace NexAI.Zendesk.Tests.Commands;

public class UpsertZendeskMembersOfRelationshipCommandTests : Neo4jDbBasedTest
{
    [Fact]
    public async Task Handle_WithUserAndGroups_CreatesMemberOfRelationsInNeo4j()
    {
        // arrange
        var userId = ZendeskUserId.New();
        var groupId1 = ZendeskGroupId.New();
        var groupId2 = ZendeskGroupId.New();

        var user = new ZendeskUser(userId, "user-123", "Test User", "test@example.com");
        var group1 = new ZendeskGroup(groupId1, "group-123", "Group 1");
        var group2 = new ZendeskGroup(groupId2, "group-456", "Group 2");

        var upsertUserCommand = new UpsertZendeskUserCommand(Neo4jDbClient);
        var upsertGroupCommand = new UpsertZendeskGroupCommand(Neo4jDbClient);
        await upsertUserCommand.Handle(user);
        await upsertGroupCommand.Handle(group1);
        await upsertGroupCommand.Handle(group2);

        var zendeskUserGroups = new ZendeskUserGroups(userId, [groupId1, groupId2]);
        var command = new UpsertZendeskMembersOfRelationshipCommand(Neo4jDbClient);

        // act
        await command.Handle(zendeskUserGroups);

        // assert
        var relationship1Record = await Neo4jDbClient.GetRelationship("User", "id", userId.ToString(), "MEMBER_OF", "Group", "id", groupId1.ToString());
        var relationship2Record = await Neo4jDbClient.GetRelationship("User", "id", userId.ToString(), "MEMBER_OF", "Group", "id", groupId2.ToString());
        relationship1Record.Should().NotBeNull();
        relationship2Record.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithMultipleCalls_DoesNotCreateDuplicateRelations()
    {
        // arrange
        var userId = ZendeskUserId.New();
        var groupId = ZendeskGroupId.New();

        var user = new ZendeskUser(userId, "user-123", "Test User", "test@example.com");
        var group = new ZendeskGroup(groupId, "group-123", "Test Group");

        var upsertUserCommand = new UpsertZendeskUserCommand(Neo4jDbClient);
        var upsertGroupCommand = new UpsertZendeskGroupCommand(Neo4jDbClient);
        await upsertUserCommand.Handle(user);
        await upsertGroupCommand.Handle(group);

        var zendeskUserGroups = new ZendeskUserGroups(userId, [groupId]);
        var command = new UpsertZendeskMembersOfRelationshipCommand(Neo4jDbClient);

        // act - call multiple times
        await command.Handle(zendeskUserGroups);
        await command.Handle(zendeskUserGroups);
        await command.Handle(zendeskUserGroups);

        // assert - should only have one relationship
        var relationshipRecord = await Neo4jDbClient.GetRelationship("User", "id", userId.ToString(), "MEMBER_OF", "Group", "id", groupId.ToString());
        relationshipRecord.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithUpdatedGroups_UpdatesRelationsInNeo4j()
    {
        // arrange
        var userId = ZendeskUserId.New();
        var groupId1 = ZendeskGroupId.New();
        var groupId2 = ZendeskGroupId.New();
        var groupId3 = ZendeskGroupId.New();

        var user = new ZendeskUser(userId, "user-123", "Test User", "test@example.com");
        var group1 = new ZendeskGroup(groupId1, "group-123", "Group 1");
        var group2 = new ZendeskGroup(groupId2, "group-456", "Group 2");
        var group3 = new ZendeskGroup(groupId3, "group-789", "Group 3");

        var upsertUserCommand = new UpsertZendeskUserCommand(Neo4jDbClient);
        var upsertGroupCommand = new UpsertZendeskGroupCommand(Neo4jDbClient);
        await upsertUserCommand.Handle(user);
        await upsertGroupCommand.Handle(group1);
        await upsertGroupCommand.Handle(group2);
        await upsertGroupCommand.Handle(group3);

        var command = new UpsertZendeskMembersOfRelationshipCommand(Neo4jDbClient);

        // act
        await command.Handle(new(userId, [groupId1, groupId2]));
        await command.Handle(new(userId, [groupId2, groupId3]));

        // assert
        var relationship1Record = await Neo4jDbClient.GetRelationship("User", "id", userId.ToString(), "MEMBER_OF", "Group", "id", groupId1.ToString());
        relationship1Record.Should().BeNull("User should no longer be member of group1");
        var relationship2Record = await Neo4jDbClient.GetRelationship("User", "id", userId.ToString(), "MEMBER_OF", "Group", "id", groupId2.ToString());
        relationship2Record.Should().NotBeNull("User should still be member of group2");
        var relationship3Record = await Neo4jDbClient.GetRelationship("User", "id", userId.ToString(), "MEMBER_OF", "Group", "id", groupId3.ToString());
        relationship3Record.Should().NotBeNull("User should be member of group3");
    }
}