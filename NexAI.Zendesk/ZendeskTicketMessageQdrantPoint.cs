using NexAI.LLMs.Common;
using Qdrant.Client.Grpc;

namespace NexAI.Zendesk;

public record ZendeskTicketMessageQdrantPoint(ZendeskTicketId TicketId, string ExternalId, ReadOnlyMemory<float> Content)
{
    public static async Task<PointStruct> Create(ZendeskTicketId zendeskTicketId, string zendeskTicketExternalId, ZendeskTicket.ZendeskTicketMessage zendeskTicketMessage, TextEmbedder textEmbedder, CancellationToken cancellationToken) =>
        new ZendeskTicketMessageQdrantPoint(
            zendeskTicketId,
            zendeskTicketExternalId,
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
                ["external_id"] = point.ExternalId
            }
        };
}