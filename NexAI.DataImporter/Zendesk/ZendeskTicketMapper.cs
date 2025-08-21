using System.Text.RegularExpressions;
using NexAI.Zendesk;
using NexAI.Zendesk.Api;

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

    public static ZendeskTicket Map(ListTicketsDto.TicketDto ticket, ListTicketCommentsDto.CommentDto[] comments, ListUsersDto.UserDto[] employees)
    {
        var description = NormalizeDescription(ticket.Description);
        var zendeskTicket = new ZendeskTicket(
            Guid.CreateVersion7(),
            NormalizeId(ticket.Id),
            NormalizeTitle(ticket.Subject, description),
            description,
            comments.Select(comment => new ZendeskTicket.ZendeskTicketMessage(
                    NormalizeCommentBody(comment.PlainBody),
                    NormalizeAuthor(comment, employees),
                    NormalizeCreateAt(comment.CreatedAt)
                )
            ).ToArray()
        );
        return zendeskTicket;
    }

    private static string NormalizeId(long? number) =>
        number is null or < 0 ? throw new("Could not parse Id") : number.Value.ToString();

    private static string NormalizeTitle(string? title, string? description)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return string.IsNullOrWhiteSpace(description) ? "<MISSING TITLE>" : description[..Math.Min(description.Length, 50)];
        }
        title = title.NormalizeText();
        return title;
    }

    private static string NormalizeDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return "<MISSING DESCRIPTION>";
        }
        description = description.NormalizeText().MaskEmailAddresses().MaskPhoneNumbers().MaskImageUrls();
        return System.Net.WebUtility.HtmlDecode(description);
    }

    private static string NormalizeCommentBody(string? comment)
    {
        if (string.IsNullOrWhiteSpace(comment))
        {
            return "<MISSING COMMENT>";
        }
        comment = comment.NormalizeText().MaskEmailAddresses().MaskPhoneNumbers().MaskImageUrls();
        return comment;
    }

    private static string NormalizeAuthor(ListTicketCommentsDto.CommentDto comment, ListUsersDto.UserDto[] employees) =>
        employees.FirstOrDefault(userDto => userDto.Id == comment.AuthorId)?.Name ?? "<EXTERNAL AUTHOR>";

    private static DateTime NormalizeCreateAt(string? createdAt) =>
        DateTime.TryParse(createdAt, out var result) ? result : throw new("Could not parse Created At");

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