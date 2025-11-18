namespace NexAI.Git.Messages;

public record GitCommitImportedEvent(Guid Id, string Sha, GitCommitImportedEvent.GitAuthor Author, string Name, string Description, DateTime CommittedAt) : IEvent
{
    public record GitAuthor(Guid Id, string Name, string Email);
}