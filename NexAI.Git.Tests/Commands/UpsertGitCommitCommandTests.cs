using FluentAssertions;
using Neo4j.Driver;
using NexAI.Git.Commands;
using NexAI.Tests.Neo4j;
using Xunit;

namespace NexAI.Git.Tests.Commands;

[Collection("Neo4j")]
public class UpsertGitCommitCommandTests(Neo4jTestFixture fixture) : Neo4jDbBasedTest(fixture)
{
    [Fact]
    public async Task Handle_WithNewCommit_CreatesCommitInNeo4j()
    {
        // arrange
        var commitId = GitCommitId.New();
        var authorId = GitAuthorId.New();
        var author = new GitAuthor(authorId, "Test Author", "test@example.com");
        var gitCommit = new GitCommit(commitId, "abc123", author, "Test Commit", "Test description", DateTime.UtcNow);
        var command = new UpsertGitCommitCommand(Neo4jDbClient);

        // act
        await command.Handle(gitCommit);

        // assert
        var commitRecord = await Neo4jDbClient.GetNode("Commit", "sha", "abc123");
        commitRecord.Should().NotBeNull();
        var commitNode = (INode)commitRecord["n"];
        ((string)commitNode["id"]).Should().Be(commitId.ToString());
        ((string)commitNode["sha"]).Should().Be("abc123");
        ((string)commitNode["name"]).Should().Be("Test Commit");
        ((string)commitNode["description"]).Should().Be("Test description");
    }

    [Fact]
    public async Task Handle_WithExistingCommit_UpdatesCommitInNeo4j()
    {
        // arrange
        var commitId = GitCommitId.New();
        var authorId = GitAuthorId.New();
        var author = new GitAuthor(authorId, "Test Author", "test@example.com");
        var gitCommit = new GitCommit(commitId, "abc123", author, "Test Commit", "Test description", DateTime.UtcNow);
        var command = new UpsertGitCommitCommand(Neo4jDbClient);

        // act - create first
        await command.Handle(gitCommit);

        // act - update
        var updatedCommit = new GitCommit(commitId, "abc123", author, "Updated Commit", "Updated description", DateTime.UtcNow);
        await command.Handle(updatedCommit);

        // assert
        var commitRecord = await Neo4jDbClient.GetNode("Commit", "sha", "abc123");
        commitRecord.Should().NotBeNull();
        var commitNode = (INode)commitRecord["n"];
        ((string)commitNode["id"]).Should().Be(commitId.ToString());
        ((string)commitNode["name"]).Should().Be("Updated Commit");
        ((string)commitNode["description"]).Should().Be("Updated description");
    }

    [Fact]
    public async Task Handle_WithMultipleCommits_CreatesAllCommitsInNeo4j()
    {
        // arrange
        var commitId1 = GitCommitId.New();
        var commitId2 = GitCommitId.New();
        var authorId1 = GitAuthorId.New();
        var authorId2 = GitAuthorId.New();
        var author1 = new GitAuthor(authorId1, "Author 1", "author1@example.com");
        var author2 = new GitAuthor(authorId2, "Author 2", "author2@example.com");
        var commit1 = new GitCommit(commitId1, "abc123", author1, "Commit 1", "Description 1", DateTime.UtcNow);
        var commit2 = new GitCommit(commitId2, "def456", author2, "Commit 2", "Description 2", DateTime.UtcNow);
        var command = new UpsertGitCommitCommand(Neo4jDbClient);

        // act
        await command.Handle(commit1);
        await command.Handle(commit2);

        // assert
        var commitRecord1 = await Neo4jDbClient.GetNode("Commit", "sha", "abc123");
        var commitRecord2 = await Neo4jDbClient.GetNode("Commit", "sha", "def456");
        commitRecord1.Should().NotBeNull();
        commitRecord2.Should().NotBeNull();
        var commitNode1 = (INode)commitRecord1["n"];
        var commitNode2 = (INode)commitRecord2["n"];
        ((string)commitNode1["name"]).Should().Be("Commit 1");
        ((string)commitNode2["name"]).Should().Be("Commit 2");
    }
}

