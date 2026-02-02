using FluentAssertions;
using FluentAssertions.Extensions;
using LibGit2Sharp;
using Xunit;

namespace NexAI.Git.Tests;

public class GitRepositoryClientTests
{
    [Fact]
    public void ExtractCommits_InvalidRepositoryPath_ThrowsArgumentException()
    {
        // arrange
        var client = new GitRepositoryClient();
        var invalidDirectory = GetTempDirectory();

        // act
        var act = () => client.ExtractCommits(invalidDirectory);

        // assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("repositoryPath")
            .WithMessage($"*Invalid git repository path: {invalidDirectory}*");
    }

    [Fact]
    public void ExtractCommits_EmptyRepository_ReturnsEmptySequence()
    {
        // arrange
        var client = new GitRepositoryClient();
        var tempDirectory = GetTempDirectory();
        Directory.CreateDirectory(tempDirectory);
        try
        {
            Repository.Init(tempDirectory);

            // act
            var commits = client.ExtractCommits(tempDirectory);

            // assert
            commits.Should().BeEmpty();
        }
        finally
        {
            TryDeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void ExtractCommits_ValidRepositoryWithCommits_ReturnsMappedCommits()
    {
        // arrange
        var client = new GitRepositoryClient();
        var tempDirectory = GetTempDirectory();
        Directory.CreateDirectory(tempDirectory);
        try
        {
            Repository.Init(tempDirectory);
            const string authorName = "Test Author";
            const string authorEmail = "test@example.com";
            const string message = "Initial commit";

            using (var repo = new Repository(tempDirectory))
            {
                var readmePath = Path.Combine(repo.Info.WorkingDirectory, "README.md");
                File.WriteAllText(readmePath, "# Test repo");
                repo.Index.Add("README.md");
                repo.Index.Write();
                var signature = new Signature(authorName, authorEmail, DateTimeOffset.UtcNow);
                repo.Commit(message, signature, signature, new CommitOptions { PrettifyMessage = false});
            }

            // act
            var commits = client.ExtractCommits(tempDirectory).ToList();

            // assert
            commits.Should().HaveCount(1);
            var commit = commits[0];
            commit.Sha.Should().NotBeNullOrEmpty();
            commit.Author.Name.Should().Be(authorName);
            commit.Author.Email.Should().Be(authorEmail);
            commit.Name.Should().Be(message);
            commit.Description.Should().Be(message);
            commit.CommittedAt.Should().BeCloseTo(DateTime.UtcNow, 1.Minutes());
        }
        finally
        {
            TryDeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void ExtractCommits_ValidRepositoryWithMultipleCommits_ReturnsAllCommitsInOrder()
    {
        // arrange
        var client = new GitRepositoryClient();
        var tempDirectory = GetTempDirectory();
        Directory.CreateDirectory(tempDirectory);
        try
        {
            Repository.Init(tempDirectory);
            using (var repo = new Repository(tempDirectory))
            {
                var signature = new Signature("Author", "author@example.com", DateTimeOffset.UtcNow);
                File.WriteAllText(Path.Combine(repo.Info.WorkingDirectory, "file1.txt"), "one");
                repo.Index.Add("file1.txt");
                repo.Index.Write();
                repo.Commit("First", signature, signature);
                File.WriteAllText(Path.Combine(repo.Info.WorkingDirectory, "file2.txt"), "two");
                repo.Index.Add("file2.txt");
                repo.Index.Write();
                repo.Commit("Second", signature, signature);
            }

            // act
            var commits = client.ExtractCommits(tempDirectory).ToList();

            // assert
            commits.Should().HaveCount(2);
            commits[0].Name.Should().Be("Second");
            commits[1].Name.Should().Be("First");
        }
        finally
        {
            TryDeleteDirectory(tempDirectory);
        }
    }

    private static string GetTempDirectory() =>
        Path.Combine(Path.GetTempPath(), "nexai", Path.GetRandomFileName());

    private static void TryDeleteDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            return;
        }
        try
        {
            Directory.Delete(path, recursive: true);
        }
        catch (UnauthorizedAccessException)
        {
            // LibGit2Sharp or OS may still hold handles; cleanup is best-effort.
        }
        catch (IOException)
        {
            // Directory in use or similar; cleanup is best-effort.
        }
    }
}