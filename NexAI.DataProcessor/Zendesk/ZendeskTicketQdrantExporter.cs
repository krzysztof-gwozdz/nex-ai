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

    public async Task CreateSchema()
    {
        AnsiConsole.MarkupLine("[yellow]Start exporting Zendesk tickets into Qdrant...[/]");
        if (_dataProcessorOptions.Recreate && await qdrantDbClient.CollectionExistsAsync(ZendeskTicketCollections.QdrantCollectionName))
        {
            await qdrantDbClient.DeleteCollectionAsync(ZendeskTicketCollections.QdrantCollectionName);
            AnsiConsole.MarkupLine("[red]Deleted collection for Zendesk tickets in Qdrant.[/]");
        }

        if (!await qdrantDbClient.CollectionExistsAsync(ZendeskTicketCollections.QdrantCollectionName))
        {
            await qdrantDbClient.CreateCollectionAsync(ZendeskTicketCollections.QdrantCollectionName, new VectorParams { Size = textEmbedder.EmbeddingDimension, Distance = Distance.Dot });
            AnsiConsole.MarkupLine("[green]Created schema for Zendesk tickets in Qdrant.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]Collection for Zendesk tickets already exists in Qdrant. Skipping schema creation.[/]");
        }
    }

    public async Task Export(ZendeskTicket zendeskTicket)
    {
        var points = new List<PointStruct>
        {
            await ZendeskTicketQdrantPoint.Create(zendeskTicket, textEmbedder),
            await ZendeskTicketTitleAndDescriptionQdrantPoint.Create(zendeskTicket, textEmbedder)
        };
        foreach (var message in zendeskTicket.Messages)
        {
            points.Add(await ZendeskTicketMessageQdrantPoint.Create(zendeskTicket.Id, message, textEmbedder));
        }
        await qdrantDbClient.UpsertAsync(ZendeskTicketCollections.QdrantCollectionName, points);
        AnsiConsole.MarkupLine("[green]Successfully exported Zendesk tickets into Qdrant.[/]");
    }
}