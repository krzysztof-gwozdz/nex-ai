using LibGit2Sharp;

namespace NexAI.Git;

public class GitRepositoryClient
{
    public IEnumerable<GitCommit> ExtractCommits(string repositoryPath)
    {
        if (!Repository.IsValid(repositoryPath))
        {
            throw new ArgumentException($"Invalid git repository path: {repositoryPath}", nameof(repositoryPath));
        }
        using var repository = new Repository(repositoryPath);
        return repository.Commits.Select(commit => GitCommit.Create(
            commit.Sha,
            GitAuthor.Create(commit.Author.Name, commit.Author.Email),
            commit.MessageShort,
            commit.Message,
            commit.Author.When.DateTime
        )).ToList();
    }
}