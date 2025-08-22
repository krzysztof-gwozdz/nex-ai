using System.Text.Json.Serialization;

namespace NexAI.Zendesk.Api.Dtos;

public record ListUsersDto(
    [property: JsonPropertyName("users")]
    UserDto[]? Users,
    string? NextPage,
    string? PreviousPage,
    int? Count) : PagedDto(NextPage, PreviousPage, Count);