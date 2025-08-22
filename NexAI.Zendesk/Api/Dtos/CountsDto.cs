using System.Text.Json.Serialization;

namespace NexAI.Zendesk.Api.Dtos;

public record CountsDto(
    [property: JsonPropertyName("count")]
    CountsDto.CountDto? Count
)
{
    public record CountDto(
        [property: JsonPropertyName("value")]
        int? Value,
        [property: JsonPropertyName("refreshed_at")]
        DateTime? RefreshedAt
    );
}