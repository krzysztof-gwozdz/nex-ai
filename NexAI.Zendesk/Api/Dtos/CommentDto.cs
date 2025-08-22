using System.Text.Json.Serialization;

namespace NexAI.Zendesk.Api.Dtos;

public record CommentDto(
    [property: JsonPropertyName("id")]
    long? Id,
    [property: JsonPropertyName("type")]
    string? Type,
    [property: JsonPropertyName("author_id")]
    long? AuthorId,
    [property: JsonPropertyName("body")]
    string? Body,
    [property: JsonPropertyName("html_body")]
    string? HtmlBody,
    [property: JsonPropertyName("plain_body")]
    string? PlainBody,
    [property: JsonPropertyName("public")]
    bool? Public,
    [property: JsonPropertyName("attachments")]
    CommentDto.AttachmentDto[]? Attachments,
    [property: JsonPropertyName("audit_id")]
    long? AuditId,
    [property: JsonPropertyName("via")]
    CommentDto.ViaDto? Via,
    [property: JsonPropertyName("created_at")]
    string? CreatedAt,
    [property: JsonPropertyName("metadata")]
    CommentDto.MetadataDto? Metadata,
    [property: JsonPropertyName("uploads")]
    string[]? Uploads)
{
    public record AttachmentDto(
        [property: JsonPropertyName("id")]
        long? Id,
        [property: JsonPropertyName("filename")]
        string? Filename,
        [property: JsonPropertyName("content_type")]
        string? ContentType,
        [property: JsonPropertyName("size")]
        long? Size,
        [property: JsonPropertyName("url")]
        string? Url);

    public record ViaDto(
        [property: JsonPropertyName("channel")]
        string? Channel,
        [property: JsonPropertyName("source")]
        ViaDto.SourceDto? Source)
    {
        public record SourceDto(
            [property: JsonPropertyName("from")]
            object? From,
            [property: JsonPropertyName("to")]
            object? To,
            [property: JsonPropertyName("rel")]
            string? Rel);
    }

    public record MetadataDto(
        [property: JsonPropertyName("system")]
        object? System,
        [property: JsonPropertyName("custom")]
        object? Custom);
}