using System.Text.Json.Serialization;

namespace NexAI.Zendesk.Api.Dtos;

public record ListTicketsDto(
    [property: JsonPropertyName("tickets")]
    TicketDto[]? Tickets,
    string? NextPage,
    string? PreviousPage,
    int? Count) : PagedDto(NextPage, PreviousPage, Count);