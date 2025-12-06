using NexAI.LLMs.Common;
using Qdrant.Client.Grpc;

namespace NexAI.Zendesk.QdrantDb;

public record ZendeskTicketTitleAndDescriptionQdrantPoint(ZendeskTicketId Id, string ExternalId, string? Level3Team, ReadOnlyMemory<float> Content)
{
    public static async Task<PointStruct> Create(ZendeskTicket zendeskTicket, TextEmbedder textEmbedder, CancellationToken cancellationToken) =>
        new ZendeskTicketTitleAndDescriptionQdrantPoint(
            zendeskTicket.Id,
            zendeskTicket.ExternalId,
            zendeskTicket.Level3Team,
            await textEmbedder.GenerateEmbedding($"{zendeskTicket.Title} | {zendeskTicket.Description}", cancellationToken)
        );

    public static implicit operator PointStruct(ZendeskTicketTitleAndDescriptionQdrantPoint point) =>
        new()
        {
            Id = Guid.NewGuid(),
            Vectors = new() { Vector = point.Content.ToArray() },
            Payload =
            {
                ["type"] = "title_and_description",
                ["ticket_id"] = point.Id.Value.ToString(),
                ["external_id"] = point.ExternalId,
                ["level3_team"] = point.Level3Team ?? string.Empty
            }
        };
}