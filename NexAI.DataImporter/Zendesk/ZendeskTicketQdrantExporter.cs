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
    private readonly QdrantOptions _qdrantOptions = options.Get<QdrantOptions>();
    private readonly TextEmbedder _textEmbedder = TextEmbedder.GetInstance(options);

    public async Task Export(ZendeskTicket[] zendeskTickets)
    {
        AnsiConsole.MarkupLine("[yellow]Start exporting Zendesk tickets into Qdrant...[/]");
        using var client = new QdrantClient(_qdrantOptions.Host, _qdrantOptions.Port);
        await CreateSchema(client);
        await InsertData(zendeskTickets, client);
    }

    private async Task CreateSchema(QdrantClient client)
    {
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
            var points = new List<PointStruct>();

            foreach (var zendeskTicket in zendeskTickets)
            {
                var embedding = await _textEmbedder.GenerateEmbedding(zendeskTicket.CombinedContent());
                var point = new PointStruct
                {
                    Id = zendeskTicket.Id == Guid.Empty ? Guid.NewGuid() : zendeskTicket.Id,
                    Vectors = new() { Vector = embedding.ToArray() },
                    Payload =
                    {
                        ["number"] = zendeskTicket.Number,
                    }
                };
                points.Add(point);
            }
            await client.UpsertAsync(ZendeskTicketCollections.QdrantCollectionName, points);
            AnsiConsole.MarkupLine("[green]Successfully exported Zendesk tickets into Qdrant.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]Zendesk tickets already exported into Qdrant. Skipping export.[/]");
        }
    }
}