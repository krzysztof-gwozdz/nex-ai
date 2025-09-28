using NexAI.LLMs.Common;
using Qdrant.Client.Grpc;

namespace NexAI.Zendesk;

public record ZendeskTicketTitleAndDescriptionQdrantPoint(ZendeskTicketId Id, string ExternalId, ReadOnlyMemory<float> Content)
{
    public static implicit operator PointStruct(ZendeskTicketTitleAndDescriptionQdrantPoint point) =>
        new()
        {
            Id = Guid.NewGuid(),
            Vectors = new() { Vector = point.Content.ToArray() },
            Payload =
            {
                ["type"] = "title_and_description",
                ["ticket_id"] = point.Id.Value.ToString(),
                ["external_id"] = point.ExternalId
            }
        };

    public static async Task<ZendeskTicketTitleAndDescriptionQdrantPoint> Create(ZendeskTicket zendeskTicket, TextEmbedder textEmbedder) =>
        new(
            zendeskTicket.Id,
            zendeskTicket.ExternalId,
            await textEmbedder.GenerateEmbedding($"{zendeskTicket.Title} | {zendeskTicket.Description}")
        );
}