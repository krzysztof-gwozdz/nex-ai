using NexAI.Config;
using NexAI.LLMs;
using NexAI.Qdrant;
using NexAI.Zendesk;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using Spectre.Console;

namespace NexAI.DataProcessor.Zendesk;

public class ZendeskTicketQdrantExporter(Options options)
{
    private readonly DataProcessorOptions _dataProcessorOptions = options.Get<DataProcessorOptions>();
    private readonly QdrantOptions _qdrantOptions = options.Get<QdrantOptions>();
    private readonly TextEmbedder _textEmbedder = TextEmbedder.GetInstance(options);

    public async Task CreateSchema()
    {
        AnsiConsole.MarkupLine("[yellow]Start exporting Zendesk tickets into Qdrant...[/]");
        using var client = new QdrantClient(new QdrantGrpcClient(_qdrantOptions.Host, _qdrantOptions.Port, _qdrantOptions.ApiKey));
        await CreateSchema(client);
    }

    public async Task Export(ZendeskTicket zendeskTicket)
    {
        using var client = new QdrantClient(new QdrantGrpcClient(_qdrantOptions.Host, _qdrantOptions.Port, _qdrantOptions.ApiKey));
        var points = new List<PointStruct>
        {
            await ZendeskTicketQdrantPoint.Create(zendeskTicket, _textEmbedder),
            await ZendeskTicketTitleAndDescriptionQdrantPoint.Create(zendeskTicket, _textEmbedder)
        };
        foreach (var message in zendeskTicket.Messages)
        {
            points.Add(await ZendeskTicketMessageQdrantPoint.Create(zendeskTicket.Id, message, _textEmbedder));
        }
        await client.UpsertAsync(ZendeskTicketCollections.QdrantCollectionName, points);
        AnsiConsole.MarkupLine("[green]Successfully exported Zendesk tickets into Qdrant.[/]");
    }

    private async Task CreateSchema(QdrantClient client)
    {
        if (_dataProcessorOptions.Recreate && await client.CollectionExistsAsync(ZendeskTicketCollections.QdrantCollectionName))
        {
            await client.DeleteCollectionAsync(ZendeskTicketCollections.QdrantCollectionName);
            AnsiConsole.MarkupLine("[red]Deleted collection for Zendesk tickets in Qdrant.[/]");
        }

        if (!await client.CollectionExistsAsync(ZendeskTicketCollections.QdrantCollectionName))
        {
            await client.CreateCollectionAsync(ZendeskTicketCollections.QdrantCollectionName, new VectorParams { Size = _textEmbedder.EmbeddingDimension, Distance = Distance.Dot });
            AnsiConsole.MarkupLine("[green]Created schema for Zendesk tickets in Qdrant.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]Collection for Zendesk tickets already exists in Qdrant. Skipping schema creation.[/]");
        }
    }
}