namespace NexAI.Git;

public record GitCommit(GitCommitId Id, string Sha, GitAuthor Author, string Name, string Description, DateTime CommittedAt)
{
    public static GitCommit Create(string sha, GitAuthor author, string name, string description, DateTime committedAt) =>
        new(GitCommitId.New(), sha, author, name, description, committedAt);
}