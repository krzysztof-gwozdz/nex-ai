using System.Text;

namespace NexAI.Zendesk;

public record ZendeskTicket(ZendeskTicketId Id, string Number, string Title, string Description, DateTime CreatedAt, DateTime? UpdatedAt, ZendeskTicket.ZendeskTicketMessage[] Messages)
{
    public record ZendeskTicketMessage(string Content, string Author, DateTime CreatedAt);
}