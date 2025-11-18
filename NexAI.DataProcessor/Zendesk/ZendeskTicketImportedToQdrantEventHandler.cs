using NexAI.LLMs.Common;
using NexAI.Qdrant;
using NexAI.Zendesk;
using NexAI.Zendesk.Messages;
using NexAI.Zendesk.QdrantDb;
using Qdrant.Client.Grpc;
using Spectre.Console;

namespace NexAI.DataProcessor.Zendesk;

public class ZendeskTicketImportedToQdrantEventHandler(QdrantDbClient qdrantDbClient, TextEmbedder textEmbedder) : IHandleMessages<ZendeskTicketImportedEvent>
{
    public async Task Handle(ZendeskTicketImportedEvent message, IMessageHandlerContext context)
    {
        var zendeskTicket = ZendeskTicket.FromZendeskTicketImportedEvent(message);
        var tasks = new List<Task<PointStruct>>
        {
            ZendeskTicketQdrantPoint.Create(zendeskTicket, textEmbedder, context.CancellationToken),
            ZendeskTicketTitleAndDescriptionQdrantPoint.Create(zendeskTicket, textEmbedder, context.CancellationToken)
        };
        tasks.AddRange(zendeskTicket.Messages.Select(zendeskTicketMessage => ZendeskTicketMessageQdrantPoint.Create(zendeskTicket, zendeskTicketMessage, textEmbedder, context.CancellationToken)));
        var points = await Task.WhenAll(tasks);
        await qdrantDbClient.UpsertAsync(ZendeskTicketQdrantCollection.Name, points, cancellationToken: context.CancellationToken);
        AnsiConsole.MarkupLine($"[mediumpurple2]Successfully exported Zendesk ticket {zendeskTicket.ExternalId} into Qdrant.[/]");
    }
}