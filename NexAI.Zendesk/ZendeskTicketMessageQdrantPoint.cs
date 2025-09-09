using NexAI.LLMs;
using Qdrant.Client.Grpc;

namespace NexAI.Zendesk;

public record ZendeskTicketMessageQdrantPoint(ZendeskTicketId Id, ReadOnlyMemory<float> Content)
{
    
    public static implicit operator PointStruct(ZendeskTicketMessageQdrantPoint point) =>
        new()
        {
            Id = Guid.NewGuid(),
            Vectors = new() { Vector = point.Content.ToArray() },
            Payload =
            {
                ["type"] = "message",
                ["ticket_id"] = point.Id.Value.ToString()
            }
        };

    public static async Task<ZendeskTicketMessageQdrantPoint> Create(ZendeskTicketId zendeskTicketId, ZendeskTicket.ZendeskTicketMessage zendeskTicketMessage, TextEmbedder textEmbedder) =>
        new(
            zendeskTicketId,
            await textEmbedder.GenerateEmbedding(zendeskTicketMessage.Content)
        );
}