using System.Text;
using NexAI.LLMs.Common;
using Qdrant.Client.Grpc;

namespace NexAI.Zendesk;

public record ZendeskTicketQdrantPoint(ZendeskTicketId Id, string ExternalId, ReadOnlyMemory<float> Content)
{
    public static async Task<PointStruct> Create(ZendeskTicket zendeskTicket, TextEmbedder textEmbedder, CancellationToken cancellationToken) =>
        new ZendeskTicketQdrantPoint(
            zendeskTicket.Id,
            zendeskTicket.ExternalId,
            await textEmbedder.GenerateEmbedding(GetCombinedContent(zendeskTicket), cancellationToken)
        );
    public static implicit operator PointStruct(ZendeskTicketQdrantPoint point) =>
        new()
        {
            Id = Guid.NewGuid(),
            Vectors = new() { Vector = point.Content.ToArray() },
            Payload =
            {
                ["type"] = "ticket",
                ["ticket_id"] = point.Id.Value.ToString(),
                ["external_id"] = point.ExternalId
            }
        };

    private static string GetCombinedContent(ZendeskTicket zendeskTicket)
    {
        var textBuilder = new StringBuilder();
        textBuilder.AppendLine(zendeskTicket.Title);
        textBuilder.AppendLine(zendeskTicket.Description);
        foreach (var message in zendeskTicket.Messages.OrderBy(message => message.CreatedAt))
        {
            textBuilder.AppendLine(message.Content);
        }
        return textBuilder.ToString();
    }
}