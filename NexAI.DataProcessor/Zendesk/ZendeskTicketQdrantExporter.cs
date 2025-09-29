using NexAI.Config;
using NexAI.LLMs.Common;
using NexAI.Qdrant;
using NexAI.Zendesk;
using Qdrant.Client.Grpc;
using Spectre.Console;

namespace NexAI.DataProcessor.Zendesk;

public class ZendeskTicketQdrantExporter(QdrantDbClient qdrantDbClient, TextEmbedder textEmbedder, Options options)
{
    private readonly DataProcessorOptions _dataProcessorOptions = options.Get<DataProcessorOptions>();

    public async Task CreateSchema(CancellationToken cancellationToken)
    {
        AnsiConsole.MarkupLine("[yellow]Start exporting Zendesk tickets into Qdrant...[/]");
        if (_dataProcessorOptions.Recreate && await qdrantDbClient.CollectionExistsAsync(ZendeskTicketCollections.QdrantCollectionName, cancellationToken))
        {
            await qdrantDbClient.DeleteCollectionAsync(ZendeskTicketCollections.QdrantCollectionName, cancellationToken: cancellationToken);
            AnsiConsole.MarkupLine("[red]Deleted collection for Zendesk tickets in Qdrant.[/]");
        }

        if (!await qdrantDbClient.CollectionExistsAsync(ZendeskTicketCollections.QdrantCollectionName, cancellationToken))
        {
            await qdrantDbClient.CreateCollectionAsync(ZendeskTicketCollections.QdrantCollectionName, new VectorParams { Size = textEmbedder.EmbeddingDimension, Distance = Distance.Dot }, cancellationToken: cancellationToken);
            AnsiConsole.MarkupLine("[green]Created schema for Zendesk tickets in Qdrant.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]Collection for Zendesk tickets already exists in Qdrant. Skipping schema creation.[/]");
        }
    }

    public async Task Export(ZendeskTicket zendeskTicket, CancellationToken cancellationToken)
    {
        var tasks = new List<Task<PointStruct>>
        {
            ZendeskTicketQdrantPoint.Create(zendeskTicket, textEmbedder, cancellationToken),
            ZendeskTicketTitleAndDescriptionQdrantPoint.Create(zendeskTicket, textEmbedder, cancellationToken)
        };
        tasks.AddRange(zendeskTicket.Messages.Select(message => ZendeskTicketMessageQdrantPoint.Create(zendeskTicket.Id, zendeskTicket.ExternalId, message, textEmbedder, cancellationToken)));
        var points = await Task.WhenAll(tasks);
        await qdrantDbClient.UpsertAsync(ZendeskTicketCollections.QdrantCollectionName, points, cancellationToken: cancellationToken);
        AnsiConsole.MarkupLine("[green]Successfully exported Zendesk tickets into Qdrant.[/]");
    }
}