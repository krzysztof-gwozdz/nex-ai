using NexAI.Git.Messages;

namespace NexAI.Git;

public record GitCommit(GitCommitId Id, string Sha, GitAuthor Author, string Name, string Description, DateTime CommittedAt)
{
    public static GitCommit Create(string sha, GitAuthor author, string name, string description, DateTime committedAt) =>
        new(GitCommitId.New(), sha, author, name, description, committedAt);
    
    public static GitCommit FromGitCommitImportedEvent(GitCommitImportedEvent gitCommitImportedEvent) =>
        new(
            new(gitCommitImportedEvent.Id),
            gitCommitImportedEvent.Sha,
            new(new(gitCommitImportedEvent.Author.Id), gitCommitImportedEvent.Author.Name, gitCommitImportedEvent.Author.Email),
            gitCommitImportedEvent.Name,
            gitCommitImportedEvent.Description,
            gitCommitImportedEvent.CommittedAt
        );

    public GitCommitImportedEvent ToGitCommitImportedEvent() =>
        new(Id.Value, Sha, new(Author.Id, Author.Name, Author.Email), Name, Description, CommittedAt);
}
