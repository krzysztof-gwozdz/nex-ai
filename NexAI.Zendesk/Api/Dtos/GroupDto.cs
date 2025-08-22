using System.Text.Json.Serialization;

namespace NexAI.Zendesk.Api.Dtos;

public record GroupDto(
    [property: JsonPropertyName("id")]
    long Id,
    [property: JsonPropertyName("url")]
    string Url,
    [property: JsonPropertyName("is_public")]
    bool? IsPublic,
    [property: JsonPropertyName("name")]
    string? Name,
    [property: JsonPropertyName("description")]
    string? Description,
    [property: JsonPropertyName("default")]
    bool Default,
    [property: JsonPropertyName("deleted")]
    bool Deleted,
    [property: JsonPropertyName("created_at")]
    string CreatedAt,
    [property: JsonPropertyName("updated_at")]
    string UpdatedAt);