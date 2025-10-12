using NexAI.LLMs.Common;
using Qdrant.Client.Grpc;

namespace NexAI.Zendesk;

public record ZendeskTicketMessageQdrantPoint(ZendeskTicketId TicketId, string ExternalId, string? Level3Team, ReadOnlyMemory<float> Content)
{
    public static async Task<PointStruct> Create(ZendeskTicket zendeskTicket, ZendeskTicket.ZendeskTicketMessage zendeskTicketMessage, TextEmbedder textEmbedder, CancellationToken cancellationToken) =>
        new ZendeskTicketMessageQdrantPoint(
            zendeskTicket.Id,
            zendeskTicket.ExternalId,
            zendeskTicket.Level3Team,
            await textEmbedder.GenerateEmbedding(zendeskTicketMessage.Content, cancellationToken)
        );

    public static implicit operator PointStruct(ZendeskTicketMessageQdrantPoint point) =>
        new()
        {
            Id = Guid.NewGuid(),
            Vectors = new() { Vector = point.Content.ToArray() },
            Payload =
            {
                ["type"] = "message",
                ["ticket_id"] = point.TicketId.Value.ToString(),
                ["external_id"] = point.ExternalId,
                ["level3_team"] = point.Level3Team ?? string.Empty
            }
        };
}