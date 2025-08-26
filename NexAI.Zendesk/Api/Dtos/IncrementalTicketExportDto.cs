using System.Text.Json.Serialization;

namespace NexAI.Zendesk.Api.Dtos;

public record IncrementalTicketExportDto(
    [property: JsonPropertyName("tickets")]
    TicketDto[]? Tickets,
    [property: JsonPropertyName("next_page")]
    string? NextPage,
    [property: JsonPropertyName("previous_page")]
    string? PreviousPage,
    [property: JsonPropertyName("end_time")]
    long? EndTime,
    [property: JsonPropertyName("end_of_stream")]
    bool? EndOfStream,
    [property: JsonPropertyName("count")]
    int? Count);