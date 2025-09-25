using NexAI.LLMs;
using Qdrant.Client.Grpc;

namespace NexAI.Zendesk;

public record ZendeskTicketTitleAndDescriptionQdrantPoint(ZendeskTicketId Id, ReadOnlyMemory<float> Content)
{
    public static implicit operator PointStruct(ZendeskTicketTitleAndDescriptionQdrantPoint point) =>
        new()
        {
            Id = point.Id.Value,
            Vectors = new() { Vector = point.Content.ToArray() },
            Payload =
            {
                ["type"] = "title_and_description",
                ["ticket_id"] = point.Id.Value.ToString()
            }
        };

    public static async Task<ZendeskTicketTitleAndDescriptionQdrantPoint> Create(ZendeskTicket zendeskTicket, TextEmbedder textEmbedder) =>
        new(
            zendeskTicket.Id,
            await textEmbedder.GenerateEmbedding($"{zendeskTicket.Title} | {zendeskTicket.Description}")
        );
}