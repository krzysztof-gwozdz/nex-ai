namespace NexAI.Git;

public record GitAuthor(GitAuthorId Id, string Name, string Email)
{
    public static GitAuthor Create(string name, string email) =>
        new(GitAuthorId.New(), name, email);
}