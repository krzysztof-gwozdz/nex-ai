using System.Text;
using NexAI.LLMs.Common;
using Qdrant.Client.Grpc;

namespace NexAI.Zendesk;

public record ZendeskTicketQdrantPoint(ZendeskTicketId Id, string ExternalId, ReadOnlyMemory<float> Content)
{
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

    public static async Task<ZendeskTicketQdrantPoint> Create(ZendeskTicket zendeskTicket, TextEmbedder textEmbedder) =>
        new(
            zendeskTicket.Id,
            zendeskTicket.ExternalId,
            await textEmbedder.GenerateEmbedding(GetCombinedContent(zendeskTicket))
        );

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