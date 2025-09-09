using NexAI.Config;
using NexAI.LLMs;
using NexAI.Qdrant;
using NexAI.Zendesk;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using Spectre.Console;

namespace NexAI.DataImporter.Zendesk;

public class ZendeskTicketQdrantExporter(Options options)
{
    private readonly DataImporterOptions _dataImporterOptions = options.Get<DataImporterOptions>();
    private readonly QdrantOptions _qdrantOptions = options.Get<QdrantOptions>();
    private readonly TextEmbedder _textEmbedder = TextEmbedder.GetInstance(options);

    public async Task Export(ZendeskTicket[] zendeskTickets)
    {
        AnsiConsole.MarkupLine("[yellow]Start exporting Zendesk tickets into Qdrant...[/]");
        using var client = new QdrantClient(new QdrantGrpcClient(_qdrantOptions.Host, _qdrantOptions.Port, _qdrantOptions.ApiKey));
        await CreateSchema(client);
        await InsertData(zendeskTickets, client);
    }

    private async Task CreateSchema(QdrantClient client)
    {
        if (_dataImporterOptions.Recreate && await client.CollectionExistsAsync(ZendeskTicketCollections.QdrantCollectionName))
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

    private async Task InsertData(ZendeskTicket[] zendeskTickets, QdrantClient client)
    {
        var collectionInfo = await client.GetCollectionInfoAsync(ZendeskTicketCollections.QdrantCollectionName);
        if (collectionInfo.VectorsCount == 0)
        {
            foreach (var zendeskTicket in zendeskTickets)
            {
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
            }
            AnsiConsole.MarkupLine("[green]Successfully exported Zendesk tickets into Qdrant.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]Zendesk tickets already exported into Qdrant. Skipping export.[/]");
        }
    }
}