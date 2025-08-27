using System.Net;
using System.Text.RegularExpressions;
using NexAI.Zendesk;
using NexAI.Zendesk.Api.Dtos;

namespace NexAI.DataImporter.Zendesk;

public static partial class ZendeskTicketMapper
{
    [GeneratedRegex("\n{2,}")]
    private static partial Regex NewLinesRegex();

    [GeneratedRegex("[\u200B-\u200D\uFEFF]")]
    private static partial Regex ZeroWidthCharsRegex();

    [GeneratedRegex(@"\b[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}\b")]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"(?<!\w)(?=(?:.*\d){9,})(?:\+?\d{1,3}[\s.-]?)?(?:\(?\d{2,4}\)?[\s.-]?){2,4}\d{2,4}\b")]
    private static partial Regex PhoneRegex();

    [GeneratedRegex(@"!\[.*?\]\([^)]+\)", RegexOptions.Singleline)]
    private static partial Regex ImageUrlRegex();

    public static ZendeskTicket Map(TicketDto ticket, CommentDto[] comments, UserDto[] employees)
    {
        var zendeskTicket = new ZendeskTicket(
            ZendeskTicketId.New(),
            NormalizeId(ticket.Id),
            NormalizeTitle(ticket.Subject),
            NormalizeDescription(ticket.Description),
            NormalizeCreatedAt(ticket.CreatedAt),
            NormalizeUpdatedAt(ticket.UpdatedAt),
            comments.Select(comment => new ZendeskTicket.ZendeskTicketMessage(
                    NormalizeCommentBody(comment.PlainBody),
                    NormalizeAuthor(comment, employees),
                    NormalizeCreatedAt(comment.CreatedAt)
                )
            ).ToArray()
        );
        return zendeskTicket;
    }

    private static string NormalizeId(long? id) =>
        id is null or < 0 ? throw new("Could not parse Id") : id.Value.ToString();

    private static string NormalizeTitle(string? subject)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            return "<MISSING TITLE>";
        }
        subject = subject.NormalizeText();
        return subject;
    }

    private static string NormalizeDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return "<MISSING DESCRIPTION>";
        }
        description = description.NormalizeText().MaskEmailAddresses().MaskPhoneNumbers().MaskImageUrls();
        return WebUtility.HtmlDecode(description);
    }

    private static string NormalizeCommentBody(string? commentBody)
    {
        if (string.IsNullOrWhiteSpace(commentBody))
        {
            return "<MISSING COMMENT>";
        }
        commentBody = commentBody.NormalizeText().MaskEmailAddresses().MaskPhoneNumbers().MaskImageUrls();
        return commentBody;
    }

    private static string NormalizeAuthor(CommentDto comment, UserDto[] employees) =>
        employees.FirstOrDefault(userDto => userDto.Id == comment.AuthorId)?.Name ?? "<NON-EMPLOYEE>";

    private static DateTime NormalizeCreatedAt(string? createdAt) =>
        DateTime.TryParse(createdAt, out var result) ? result : throw new("Could not parse Created At");
    
    private static DateTime? NormalizeUpdatedAt(string? updatedAt) =>
        updatedAt is null ? null : DateTime.TryParse(updatedAt, out var result) ? result : throw new("Could not Updated At");

    private static string NormalizeText(this string text) =>
        text.NormalizeNewLines()
            .RemoveZeroWidthChars()
            .Trim();

    private static string NormalizeNewLines(this string text) =>
        NewLinesRegex().Replace(text, "\n");

    private static string RemoveZeroWidthChars(this string text) =>
        ZeroWidthCharsRegex().Replace(text, "");

    private static string MaskEmailAddresses(this string text) =>
        EmailRegex().Replace(text, "<EMAIL ADDRESS>");

    private static string MaskPhoneNumbers(this string text) =>
        PhoneRegex().Replace(text, "<PHONE NUMBER>");

    private static string MaskImageUrls(this string text) =>
        ImageUrlRegex().Replace(text, "<IMAGE URL>");
}