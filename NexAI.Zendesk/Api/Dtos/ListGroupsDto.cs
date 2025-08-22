using System.Text.Json.Serialization;

namespace NexAI.Zendesk.Api.Dtos;

public record ListGroupsDto(
    [property: JsonPropertyName("groups")]
    GroupDto[] Groups,
    string? NextPage,
    string? PreviousPage,
    int? Count) : PagedDto(NextPage, PreviousPage, Count);