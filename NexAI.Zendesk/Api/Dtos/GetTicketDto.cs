using System.Text.Json.Serialization;

namespace NexAI.Zendesk.Api.Dtos;

public record GetTicketDto([property: JsonPropertyName("ticket")] TicketDto? Ticket);