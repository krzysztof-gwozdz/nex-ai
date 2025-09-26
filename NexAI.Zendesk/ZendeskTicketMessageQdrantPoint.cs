using NexAI.LLMs.Common;
using Qdrant.Client.Grpc;

namespace NexAI.Zendesk;

public record ZendeskTicketMessageQdrantPoint(ZendeskTicketMessageId Id, ZendeskTicketId TicketId, ReadOnlyMemory<float> Content)
{
    
    public static implicit operator PointStruct(ZendeskTicketMessageQdrantPoint point) =>
        new()
        {
            Id = point.Id.Value,
            Vectors = new() { Vector = point.Content.ToArray() },
            Payload =
            {
                ["type"] = "message",
                ["ticket_id"] = point.TicketId.Value.ToString()
            }
        };

    public static async Task<ZendeskTicketMessageQdrantPoint> Create(ZendeskTicketId zendeskTicketId, ZendeskTicket.ZendeskTicketMessage zendeskTicketMessage, TextEmbedder textEmbedder) =>
        new(
            zendeskTicketMessage.Id,
            zendeskTicketId,
            await textEmbedder.GenerateEmbedding(zendeskTicketMessage.Content)
        );
}