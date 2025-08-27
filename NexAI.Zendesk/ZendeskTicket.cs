using System.Text;

namespace NexAI.Zendesk;

public record ZendeskTicket(ZendeskTicketId Id, string Number, string Title, string Description, DateTime CreatedAt, DateTime? UpdatedAt, ZendeskTicket.ZendeskTicketMessage[] Messages)
{
    public record ZendeskTicketMessage(string Content, string Author, DateTime CreatedAt);

    public string CombinedContent()
    {
        var textBuilder = new StringBuilder();
        textBuilder.AppendLine(Title);
        textBuilder.AppendLine(Description);
        foreach (var message in Messages.OrderBy(message => message.CreatedAt))
        {
            textBuilder.AppendLine(message.Content);
        }

        return textBuilder.ToString();
    }
}