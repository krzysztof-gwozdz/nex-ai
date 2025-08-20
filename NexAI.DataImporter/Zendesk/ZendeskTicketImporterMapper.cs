using NexAI.Zendesk;
using NexAI.Zendesk.Api;

namespace NexAI.DataImporter.Zendesk;

public static class ZendeskTicketImporterMapper
{
    public static ZendeskTicket Map(ListTicketsDto.TicketDto ticket, ListTicketCommentsDto.CommentDto[] comments, ListUsersDto.UserDto[] employees)
    {
        return new(
            Guid.CreateVersion7(),
            ticket.Id!.Value.ToString(),
            ticket.Subject ?? "<MISSING TITLE>",
            ticket.Description ?? "<MISSING DESCRIPTION>",
            comments.Select(comment => new ZendeskTicket.ZendeskTicketMessage(
                    comment.PlainBody ?? "<MISSING BODY>",
                    employees.FirstOrDefault(e => e.Id == comment.AuthorId)?.Name ?? "Unknown Author",
                    DateTime.Parse(comment.CreatedAt ?? "<MISSING CREATED AT>")
                )
            ).ToArray()
        );
    }
}