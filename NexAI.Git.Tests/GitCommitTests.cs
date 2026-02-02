using FluentAssertions;
using NexAI.Git.Messages;
using Xunit;

namespace NexAI.Git.Tests;

public class GitCommitTests
{
    [Fact]
    public void Constructor_AssignsAllProperties()
    {
        // arrange
        var id = GitCommitId.New();
        const string sha = "abc123def456";
        var author = GitAuthor.Create("Jane Doe", "jane@example.com");
        const string name = "feat: add feature";
        const string description = "Detailed description";
        var committedAt = new DateTime(2025, 2, 1, 10, 0, 0, DateTimeKind.Utc);

        // act
        var commit = new GitCommit(id, sha, author, name, description, committedAt);

        // assert
        commit.Id.Should().Be(id);
        commit.Sha.Should().Be(sha);
        commit.Author.Should().Be(author);
        commit.Name.Should().Be(name);
        commit.Description.Should().Be(description);
        commit.CommittedAt.Should().Be(committedAt);
    }

    [Fact]
    public void Constructor_WithSameValues_ProducesEqualRecords()
    {
        // arrange
        var id = GitCommitId.New();
        var author = GitAuthor.Create("Same Author", "same@example.com");
        var committedAt = new DateTime(2025, 2, 1, 10, 0, 0, DateTimeKind.Utc);
        var commit1 = new GitCommit(id, "sha123", author, "Same Name", "Same Description", committedAt);
        var commit2 = new GitCommit(id, "sha123", author, "Same Name", "Same Description", committedAt);

        // act & assert
        commit1.Should().Be(commit2);
        (commit1 == commit2).Should().BeTrue();
    }

    [Fact]
    public void Create_ReturnsCommitWithGivenShaAuthorNameDescriptionAndCommittedAt()
    {
        // arrange
        const string sha = "abc123";
        var author = GitAuthor.Create("John Smith", "john@example.com");
        const string name = "fix: bug fix";
        const string description = "Fixes the issue";
        var committedAt = new DateTime(2025, 1, 15, 14, 30, 0, DateTimeKind.Utc);

        // act
        var commit = GitCommit.Create(sha, author, name, description, committedAt);

        // assert
        commit.Sha.Should().Be(sha);
        commit.Author.Should().Be(author);
        commit.Name.Should().Be(name);
        commit.Description.Should().Be(description);
        commit.CommittedAt.Should().Be(committedAt);
        commit.Id.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_EachCall_GeneratesNewId()
    {
        // arrange & act
        var author = GitAuthor.Create("Author", "author@example.com");
        var committedAt = DateTime.UtcNow;
        var commit1 = GitCommit.Create("sha1", author, "Name 1", "Desc 1", committedAt);
        var commit2 = GitCommit.Create("sha2", author, "Name 2", "Desc 2", committedAt);

        // assert
        commit1.Id.Value.Should().NotBe(commit2.Id.Value);
    }

    [Fact]
    public void FromGitCommitImportedEvent_ReturnsCommitWithEventData()
    {
        // arrange
        var commitId = Guid.NewGuid();
        const string sha = "event-sha-789";
        var authorId = Guid.NewGuid();
        const string authorName = "Event Author";
        const string authorEmail = "event@example.com";
        const string name = "event: name";
        const string description = "Event description";
        var committedAt = new DateTime(2025, 2, 2, 12, 0, 0, DateTimeKind.Utc);
        var evt = new GitCommitImportedEvent(
            commitId,
            sha,
            new GitCommitImportedEvent.GitAuthor(authorId, authorName, authorEmail),
            name,
            description,
            committedAt);

        // act
        var commit = GitCommit.FromGitCommitImportedEvent(evt);

        // assert
        commit.Id.Value.Should().Be(commitId);
        commit.Sha.Should().Be(sha);
        commit.Author.Id.Value.Should().Be(authorId);
        commit.Author.Name.Should().Be(authorName);
        commit.Author.Email.Should().Be(authorEmail);
        commit.Name.Should().Be(name);
        commit.Description.Should().Be(description);
        commit.CommittedAt.Should().Be(committedAt);
    }

    [Fact]
    public void ToGitCommitImportedEvent_ReturnsEventWithCommitData()
    {
        // arrange
        var commit = GitCommit.Create(
            "sha-to-event",
            GitAuthor.Create("To Event", "toevent@example.com"),
            "event name",
            "event description",
            new DateTime(2025, 2, 2, 15, 0, 0, DateTimeKind.Utc));

        // act
        var evt = commit.ToGitCommitImportedEvent();

        // assert
        evt.Id.Should().Be(commit.Id.Value);
        evt.Sha.Should().Be(commit.Sha);
        evt.Author.Id.Should().Be(commit.Author.Id.Value);
        evt.Author.Name.Should().Be(commit.Author.Name);
        evt.Author.Email.Should().Be(commit.Author.Email);
        evt.Name.Should().Be(commit.Name);
        evt.Description.Should().Be(commit.Description);
        evt.CommittedAt.Should().Be(commit.CommittedAt);
    }
}
