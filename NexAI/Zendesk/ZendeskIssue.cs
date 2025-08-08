namespace NexAI.Zendesk;

public record ZendeskIssue(string Id, string Title, string Description, ZendeskIssue.ZendeskIssueMessage[] Messages)
{
    public record ZendeskIssueMessage(string Content, string Author, DateTime CreatedAt);
}