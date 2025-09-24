using System.Net;
using System.Text.RegularExpressions;
using NexAI.Zendesk.Api.Dtos;

namespace NexAI.Zendesk;

public static partial class ZendeskTicketMapper
{
    [GeneratedRegex(@"(\r?\n){2,}")]
    private static partial Regex NewLinesRegex();

    [GeneratedRegex("[\u200B-\u200D\uFEFF ]")]
    private static partial Regex ZeroWidthCharsRegex();

    [GeneratedRegex(@"\b[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}\b")]
    private static partial Regex EmailAddressRegex();

    [GeneratedRegex(@"(?<!\w)(?=(?:.*\d){9,})(?:\+?\d{1,3}[\s.-]?)?(?:\(?\d{2,4}\)?[\s.-]?){2,4}\d{2,4}\b")]
    private static partial Regex PhoneNumberRegex();

    [GeneratedRegex(@"!\[.*?\]\([^)]+\)", RegexOptions.Singleline)]
    private static partial Regex ImageUrlRegex();

    public static ZendeskTicket Map(TicketDto ticket, CommentDto[] comments, UserDto[] employees)
    {
        var zendeskTicket = new ZendeskTicket(
            ZendeskTicketId.New(),
            NormalizeNumber(ticket.Id),
            NormalizeTitle(ticket.Subject),
            NormalizeDescription(ticket.Description),
            NormalizeUrl(ticket.Url),
            NormalizeCategory(ticket.CustomFields),
            NormalizeStatus(ticket.Status),
            NormalizeCountry(ticket.CustomFields),
            NormalizeMerchantId(ticket.CustomFields),
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

    private static string NormalizeNumber(long? id) =>
        id is null or < 0 ? throw new("Could not parse Id") : id.Value.ToString();

    private static string NormalizeTitle(string? subject)
    {
        var title = subject?.NormalizeText();
        return string.IsNullOrWhiteSpace(title) ? "<MISSING TITLE>" : title;
    }

    private static string NormalizeDescription(string? description)
    {
        description = description?.NormalizeText().MaskEmailAddresses().MaskPhoneNumbers().MaskImageUrls();
        return string.IsNullOrWhiteSpace(description) ? "<MISSING DESCRIPTION>" : WebUtility.HtmlDecode(description);
    }

    private static string NormalizeUrl(string? url) =>
        string.IsNullOrWhiteSpace(url) ? "<MISSING URL>" : url;

    private static string NormalizeCategory(TicketDto.CustomField[]? customFields)
    {
        var category = customFields?.FirstOrDefault(customField => customField.Id == 23426028)?.Value?.ToString();
        return string.IsNullOrWhiteSpace(category) ? string.Empty : category;
    }

    private static string NormalizeCountry(TicketDto.CustomField[]? customFields)
    {
        var country = customFields?.FirstOrDefault(customField => customField.Id == 360000060007)?.Value?.ToString();
        return string.IsNullOrWhiteSpace(country) ? string.Empty : country;
    }

    private static string NormalizeMerchantId(TicketDto.CustomField[]? customFields)
    {
        var merchantId = customFields?.FirstOrDefault(customField => customField.Id == 21072413)?.Value?.ToString();
        return string.IsNullOrWhiteSpace(merchantId) || merchantId == "0" || merchantId == "00" ? string.Empty : merchantId;
    }

    private static string NormalizeStatus(string? status) =>
        string.IsNullOrWhiteSpace(status) ? "<MISSING STATUS>" : status;

    private static string NormalizeCommentBody(string? commentBody)
    {
        var commentContent = commentBody?.NormalizeText().MaskEmailAddresses().MaskPhoneNumbers().MaskImageUrls();
        return string.IsNullOrWhiteSpace(commentContent) ? string.Empty : commentContent;
    }

    private static string NormalizeAuthor(CommentDto comment, UserDto[] employees) =>
        comment.AuthorId is null or < 0 ? "<UNKNOWN AUTHOR>" : employees.FirstOrDefault(userDto => userDto.Id == comment.AuthorId)?.Name ?? "<NON-EMPLOYEE>";

    private static DateTime NormalizeCreatedAt(string? createdAt) =>
        DateTime.TryParse(createdAt, out var result) ? result : throw new("Could not parse Created At");

    private static DateTime? NormalizeUpdatedAt(string? updatedAt) =>
        updatedAt is null ? null : DateTime.TryParse(updatedAt, out var result) ? result : throw new("Could not Updated At");

    private static string NormalizeText(this string text) =>
        text.RemoveZeroWidthChars()
            .NormalizeNewLines()
            .Trim();

    private static string NormalizeNewLines(this string text) =>
        NewLinesRegex().Replace(text, "\n");

    private static string RemoveZeroWidthChars(this string text) =>
        ZeroWidthCharsRegex().Replace(text, "");

    private static string MaskEmailAddresses(this string text) =>
        EmailAddressRegex().Replace(text, "<EMAIL ADDRESS>");

    private static string MaskPhoneNumbers(this string text) =>
        PhoneNumberRegex().Replace(text, "<PHONE NUMBER>");

    private static string MaskImageUrls(this string text) =>
        ImageUrlRegex().Replace(text, "<IMAGE URL>");
}