using System.Text.Json.Serialization;

namespace NexAI.Zendesk.Api.Dtos;

public record ListTicketCommentsDto(
    [property: JsonPropertyName("comments")]
    CommentDto[]? Comments,
    string? NextPage,
    string? PreviousPage,
    int? Count) : PagedDto(NextPage, PreviousPage, Count);