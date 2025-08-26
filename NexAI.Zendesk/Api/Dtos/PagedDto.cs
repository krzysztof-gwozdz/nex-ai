using System.Text.Json.Serialization;

namespace NexAI.Zendesk.Api.Dtos;

public record PagedDto(
    [property: JsonPropertyName("next_page")]
    string? NextPage,
    [property: JsonPropertyName("previous_page")]
    string? PreviousPage,
    [property: JsonPropertyName("count")]
    int? Count);