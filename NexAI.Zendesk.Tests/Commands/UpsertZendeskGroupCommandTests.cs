using FluentAssertions;
using Neo4j.Driver;
using NexAI.Tests;
using NexAI.Zendesk.Commands;
using Xunit;

namespace NexAI.Zendesk.Tests.Commands;

public class UpsertZendeskGroupCommandTests : TestBase
{
    [Fact]
    public async Task Handle_WithNewGroup_CreatesGroupInNeo4j()
    {
        // arrange
        var groupId = ZendeskGroupId.New();
        var zendeskGroup = new ZendeskGroup(groupId, "group-123", "Test Group");
        var command = new UpsertZendeskGroupCommand(Neo4jDbClient);

        // act
        await command.Handle(zendeskGroup);

        // assert
        var groupRecord = await Neo4jDbClient.GetNode("Group", "zendeskId", "group-123");
        groupRecord.Should().NotBeNull();
        var groupNode = (INode)groupRecord["n"];
        ((string)groupNode["id"]).Should().Be(groupId.ToString());
        ((string)groupNode["zendeskId"]).Should().Be("group-123");
        ((string)groupNode["name"]).Should().Be("Test Group");
    }

    [Fact]
    public async Task Handle_WithExistingGroup_UpdatesGroupInNeo4j()
    {
        // arrange
        var groupId = ZendeskGroupId.New();
        var zendeskGroup = new ZendeskGroup(groupId, "group-123", "Test Group");
        var command = new UpsertZendeskGroupCommand(Neo4jDbClient);

        // act - create first
        await command.Handle(zendeskGroup);

        // act - update
        var updatedGroup = new ZendeskGroup(groupId, "group-123", "Updated Group");
        await command.Handle(updatedGroup);

        // assert
        var groupRecord = await Neo4jDbClient.GetNode("Group", "zendeskId", "group-123");
        groupRecord.Should().NotBeNull();
        var groupNode = (INode)groupRecord["n"];
        ((string)groupNode["id"]).Should().Be(groupId.ToString());
        ((string)groupNode["name"]).Should().Be("Updated Group");
    }

    [Fact]
    public async Task Handle_WithMultipleGroups_CreatesAllGroupsInNeo4j()
    {
        // arrange
        var groupId1 = ZendeskGroupId.New();
        var groupId2 = ZendeskGroupId.New();
        var group1 = new ZendeskGroup(groupId1, "group-123", "Group 1");
        var group2 = new ZendeskGroup(groupId2, "group-456", "Group 2");
        var command = new UpsertZendeskGroupCommand(Neo4jDbClient);

        // act
        await command.Handle(group1);
        await command.Handle(group2);

        // assert
        var groupRecord1 = await Neo4jDbClient.GetNode("Group", "zendeskId", "group-123");
        var groupRecord2 = await Neo4jDbClient.GetNode("Group", "zendeskId", "group-456");
        groupRecord1.Should().NotBeNull();
        groupRecord2.Should().NotBeNull();
        var groupNode1 = (INode)groupRecord1["n"];
        var groupNode2 = (INode)groupRecord2["n"];
        ((string)groupNode1["name"]).Should().Be("Group 1");
        ((string)groupNode2["name"]).Should().Be("Group 2");
    }
}