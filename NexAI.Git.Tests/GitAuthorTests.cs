using FluentAssertions;
using Xunit;

namespace NexAI.Git.Tests;

public class GitAuthorTests
{
    [Fact]
    public void Constructor_AssignsAllProperties()
    {
        // arrange
        var id = GitAuthorId.New();
        const string name = "Jane Doe";
        const string email = "jane@example.com";

        // act
        var author = new GitAuthor(id, name, email);

        // assert
        author.Id.Should().Be(id);
        author.Name.Should().Be(name);
        author.Email.Should().Be(email);
    }

    [Fact]
    public void Constructor_WithSameValues_ProducesEqualRecords()
    {
        // arrange
        var id = GitAuthorId.New();
        var author1 = new GitAuthor(id, "Same Name", "same@example.com");
        var author2 = new GitAuthor(id, "Same Name", "same@example.com");

        // act & assert
        author1.Should().Be(author2);
        (author1 == author2).Should().BeTrue();
    }

    [Fact]
    public void Create_ReturnsAuthorWithGivenNameAndEmail()
    {
        // arrange
        const string name = "John Smith";
        const string email = "john@example.com";

        // act
        var author = GitAuthor.Create(name, email);

        // assert
        author.Name.Should().Be(name);
        author.Email.Should().Be(email);
        author.Id.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_EachCall_GeneratesNewId()
    {
        // arrange & act
        var author1 = GitAuthor.Create("Author One", "one@example.com");
        var author2 = GitAuthor.Create("Author Two", "two@example.com");

        // assert
        author1.Id.Value.Should().NotBe(author2.Id.Value);
    }
}