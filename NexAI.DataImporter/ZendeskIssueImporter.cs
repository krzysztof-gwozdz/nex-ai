using System.Text.Json;
using NexAI.Config;
using NexAI.OpenAI;
using NexAI.Qdrant;
using NexAI.Zendesk;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using Spectre.Console;

namespace NexAI.DataImporter;

internal class ZendeskIssueImporter(Options options)
{
    private const string CollectionName = "nexai.zendesk_issues";
    private readonly QdrantOptions _qdrantOptions = options.Get<QdrantOptions>();
    private readonly TextEmbedder _textEmbedder = new(options);

    public async Task Initialize()
    {
        AnsiConsole.MarkupLine("[yellow]Initializing Zendesk issue store...[/]");
        using var client = new QdrantClient(_qdrantOptions.Host, _qdrantOptions.Port);
        if (!await client.CollectionExistsAsync(CollectionName))
        {
            await client.CreateCollectionAsync(CollectionName, new VectorParams { Size = 1536, Distance = Distance.Dot });
            await PopulateSampleData();
            AnsiConsole.MarkupLine("[green]Zendesk issue store initialized with sample data.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[green]Zendesk issue store already initialized.[/]");
        }
    }

    private async Task PopulateSampleData()
    {
        var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample-issues.json");
        if (!File.Exists(jsonPath))
        {
            throw new($"Sample issues file not found at: {jsonPath}");
        }

        var jsonContent = await File.ReadAllTextAsync(jsonPath);
        var zendeskIssues = JsonSerializer.Deserialize<ZendeskIssue[]>(jsonContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        if (zendeskIssues == null)
        {
            throw new("Failed to deserialize sample issues from JSON");
        }

        using var client = new QdrantClient(_qdrantOptions.Host, _qdrantOptions.Port);
        var points = new List<PointStruct>();

        foreach (var zendeskIssue in zendeskIssues)
        {
            var embedding = await _textEmbedder.GenerateEmbedding(zendeskIssue.ToString());
            var point = new PointStruct
            {
                Id = zendeskIssue.Id == Guid.Empty ? Guid.NewGuid() : zendeskIssue.Id,
                Vectors = new() { Vector = embedding.ToArray() },
                Payload =
                {
                    ["number"] = zendeskIssue.Number,
                    ["title"] = zendeskIssue.Title,
                    ["description"] = zendeskIssue.Description,
                    ["messages"] = JsonSerializer.Serialize(zendeskIssue.Messages),
                }
            };
            points.Add(point);
        }

        await client.UpsertAsync(CollectionName, points);
    }
}