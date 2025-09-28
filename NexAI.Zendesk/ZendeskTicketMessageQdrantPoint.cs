using NexAI.LLMs.Common;
using Qdrant.Client.Grpc;

namespace NexAI.Zendesk;

public record ZendeskTicketMessageQdrantPoint(ZendeskTicketId TicketId, string ExternalId, ReadOnlyMemory<float> Content)
{
    public static implicit operator PointStruct(ZendeskTicketMessageQdrantPoint point) =>
        new()
        {
            Id = Guid.NewGuid(),
            Vectors = new() { Vector = point.Content.ToArray() },
            Payload =
            {
                ["type"] = "message",
                ["ticket_id"] = point.TicketId.Value.ToString(),
                ["external_id"] = point.ExternalId
            }
        };

    public static async Task<ZendeskTicketMessageQdrantPoint> Create(ZendeskTicketId zendeskTicketId, string zendeskTicketExternalId, ZendeskTicket.ZendeskTicketMessage zendeskTicketMessage, TextEmbedder textEmbedder) =>
        new(
            zendeskTicketId,
            zendeskTicketExternalId,
            await textEmbedder.GenerateEmbedding(zendeskTicketMessage.Content)
        );
}