using FluentAssertions;
using Neo4j.Driver;
using NexAI.Tests.Neo4j;
using NexAI.Zendesk.Commands;
using Xunit;

namespace NexAI.Zendesk.Tests.Commands;

[Collection("Neo4j")]
public class UpsertZendeskUserCommandTests(Neo4jTestFixture fixture) : Neo4jDbBasedTest(fixture)
{
    [Fact]
    public async Task Handle_WithNewUser_CreatesUserInNeo4j()
    {
        // arrange
        var userId = ZendeskUserId.New();
        var zendeskUser = new ZendeskUser(userId, "12345", "Test User", "test@example.com");
        var command = new UpsertZendeskUserCommand(Neo4jDbClient);

        // act
        await command.Handle(zendeskUser);

        // assert
        var userRecord = await Neo4jDbClient.GetNode("User", "zendeskId", "12345");
        userRecord.Should().NotBeNull();
        var userNode = (INode)userRecord["n"];
        ((string)userNode["id"]).Should().Be(userId.ToString());
        ((string)userNode["zendeskId"]).Should().Be("12345");
        ((string)userNode["name"]).Should().Be("Test User");
        ((string)userNode["email"]).Should().Be("test@example.com");
    }

    [Fact]
    public async Task Handle_WithExistingUser_UpdatesUserInNeo4j()
    {
        // arrange
        var userId = ZendeskUserId.New();
        var zendeskUser = new ZendeskUser(userId, "12345", "Test User", "test@example.com");
        var command = new UpsertZendeskUserCommand(Neo4jDbClient);

        // act - create first
        await command.Handle(zendeskUser);

        // act - update
        var updatedUser = new ZendeskUser(userId, "12345", "Updated User", "updated@example.com");
        await command.Handle(updatedUser);

        // assert
        var userRecord = await Neo4jDbClient.GetNode("User", "zendeskId", "12345");
        userRecord.Should().NotBeNull();
        var userNode = (INode)userRecord["n"];
        ((string)userNode["id"]).Should().Be(userId.ToString());
        ((string)userNode["name"]).Should().Be("Updated User");
        ((string)userNode["email"]).Should().Be("updated@example.com");
    }

    [Fact]
    public async Task Handle_WithMultipleUsers_CreatesAllUsersInNeo4j()
    {
        // arrange
        var userId1 = ZendeskUserId.New();
        var userId2 = ZendeskUserId.New();
        var user1 = new ZendeskUser(userId1, "12345", "User 1", "user1@example.com");
        var user2 = new ZendeskUser(userId2, "67890", "User 2", "user2@example.com");
        var command = new UpsertZendeskUserCommand(Neo4jDbClient);

        // act
        await command.Handle(user1);
        await command.Handle(user2);

        // assert
        var userRecord1 = await Neo4jDbClient.GetNode("User", "zendeskId", "12345");
        var userRecord2 = await Neo4jDbClient.GetNode("User", "zendeskId", "67890");
        userRecord1.Should().NotBeNull();
        userRecord2.Should().NotBeNull();
        var userNode1 = (INode)userRecord1["n"];
        var userNode2 = (INode)userRecord2["n"];
        ((string)userNode1["name"]).Should().Be("User 1");
        ((string)userNode2["name"]).Should().Be("User 2");
    }
}