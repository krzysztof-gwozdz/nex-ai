using NexAI.Config;
using NexAI.OpenAI;
using NexAI.Qdrant;
using NexAI.Zendesk;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using Spectre.Console;

namespace NexAI.DataImporter.Zendesk;

public class ZendeskIssueQdrantExporter(Options options)
{
    private readonly TextEmbedder _textEmbedder = new(options);
    private readonly QdrantOptions _qdrantOptions = options.Get<QdrantOptions>();

    public async Task Export(ZendeskIssue[] zendeskIssues)
    {
        AnsiConsole.MarkupLine("[yellow]Start exporting Zendesk issues into Qdrant...[/]");
        using var client = new QdrantClient(_qdrantOptions.Host, _qdrantOptions.Port);
        if (!await client.CollectionExistsAsync(ZendeskIssueCollections.QdrantCollectionName))
        {
            await client.CreateCollectionAsync(ZendeskIssueCollections.QdrantCollectionName, new VectorParams { Size = 1536, Distance = Distance.Dot });
            await InsertData(zendeskIssues);
            AnsiConsole.MarkupLine("[green]Zendesk issue store initialized.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[green]Zendesk issue already initialized.[/]");
        }
    }

    private async Task InsertData(ZendeskIssue[] zendeskIssues)
    {
        using var client = new QdrantClient(_qdrantOptions.Host, _qdrantOptions.Port);
        var points = new List<PointStruct>();

        foreach (var zendeskIssue in zendeskIssues)
        {
            var embedding = await _textEmbedder.GenerateEmbedding(zendeskIssue.CombinedContent());
            var point = new PointStruct
            {
                Id = zendeskIssue.Id == Guid.Empty ? Guid.NewGuid() : zendeskIssue.Id,
                Vectors = new() { Vector = embedding.ToArray() },
                Payload =
                {
                    ["number"] = zendeskIssue.Number,
                }
            };
            points.Add(point);
        }
        await client.UpsertAsync(ZendeskIssueCollections.QdrantCollectionName, points);
    }
}