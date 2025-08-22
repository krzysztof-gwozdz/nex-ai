using NexAI.LLMs;
using Qdrant.Client.Grpc;

namespace NexAI.Zendesk;

public record ZendeskTicketQdrantPoint(Guid Id, string Number, ReadOnlyMemory<float> Content)
{
    public PointStruct ToPointStruct() =>
        new()
        {
            Id = Id,
            Vectors = new() { Vector = Content.ToArray() },
            Payload =
            {
                ["number"] = Number,
            }
        };

    public static async Task<ZendeskTicketQdrantPoint> Create(ZendeskTicket zendeskTicket, TextEmbedder textEmbedder) =>
        new(
            zendeskTicket.Id,
            zendeskTicket.Number,
            await textEmbedder.GenerateEmbedding(zendeskTicket.CombinedContent())
        );
}