using System.Text;

namespace NexAI.Zendesk;

public record ZendeskIssue(Guid Id, string Number, string Title, string Description, ZendeskIssue.ZendeskIssueMessage[] Messages)
{
    public record ZendeskIssueMessage(string Content, string Author, DateTime CreatedAt);

    public override string ToString()
    {
        var textBuilder = new StringBuilder();
        textBuilder.AppendLine(Title ?? "");
        textBuilder.AppendLine(Description ?? "");
        foreach (var message in Messages.OrderBy(m => m.CreatedAt))
        {
            textBuilder.AppendLine(message.Content ?? "");
        }

        return textBuilder.ToString();
    }
}