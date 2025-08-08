using System.Text;
using System.Text.Json;
using NexAI.Config;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using Spectre.Console;

namespace NexAI.Zendesk;

public class ZendeskIssueStore(Options options)
{
    private const string CollectionName = "nexai.zendesk_issues";
    private readonly AI _ai = new(options);
    private readonly QdrantOptions _qdrantOptions = options.Get<QdrantOptions>();
    
    private bool _initialized;

    public async Task<ZendeskIssue?> GetIssueByNumber(string number)
    {
        using var client = new QdrantClient(_qdrantOptions.Host, _qdrantOptions.Port);
        var response = await client.ScrollAsync(CollectionName, filter: new()
        {
            Must =
            {
                new Condition
                {
                    Field = new()
                    {
                        Key = "number",
                        Match = new() { Text = number }
                    }
                }
            }
        }, limit: 1);
        
        if (response.Result.Count == 0)
            return null;

        var point = response.Result.First();
        return new(
            point.Payload["number"].StringValue,
            point.Payload["title"].StringValue,
            point.Payload["description"].StringValue,
            JsonSerializer.Deserialize<ZendeskIssue.ZendeskIssueMessage[]>(point.Payload["messages"].StringValue) ?? []
        );
    }

    public async Task<List<SimilarIssue>> FindSimilarIssuesByNumber(string issueId, ulong limit)
    {
        var targetIssue = await GetIssueByNumber(issueId);
        if (targetIssue == null)
            return [];

        var targetText = BuildIssueText(targetIssue);
        return (await FindSimilarIssuesByPhrase(targetText, limit + 1))
            .Where(issue => issue.Number != issueId)
            .ToList();
    }

    public async Task<List<SimilarIssue>> FindSimilarIssuesByPhrase(string text, ulong limit)
    {
        using var client = new QdrantClient(_qdrantOptions.Host, _qdrantOptions.Port);
        var embedding = await _ai.GenerateEmbedding(text);
        var response = await client.SearchAsync(CollectionName, embedding, limit: limit);

        return response
            .Select(point => new SimilarIssue(
                point.Payload["number"].StringValue,
                point.Payload["title"].StringValue,
                point.Score)
            )
            .ToList();
    }

    private static string BuildIssueText(ZendeskIssue issue)
    {
        var textBuilder = new StringBuilder();
        textBuilder.AppendLine(issue.Title ?? "");
        textBuilder.AppendLine(issue.Description ?? "");
        foreach (var message in issue.Messages.OrderBy(m => m.CreatedAt))
        {
            textBuilder.AppendLine(message.Content ?? "");
        }

        return textBuilder.ToString();
    }

    public async Task Initialize()
    {
        if (!_initialized)
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
            _initialized = true;
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]Zendesk issue store is already initialized.[/]");
        }
    }

    private async Task PopulateSampleData()
    {
        var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Zendesk", "sample-issues.json");
        if (!File.Exists(jsonPath))
        {
            throw new SomethingIsNotYesException($"Sample issues file not found at: {jsonPath}");
        }

        var jsonContent = await File.ReadAllTextAsync(jsonPath);
        var issues = JsonSerializer.Deserialize<ZendeskIssue[]>(jsonContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        if (issues == null)
        {
            throw new SomethingIsNotYesException("Failed to deserialize sample issues from JSON");
        }

        using var client = new QdrantClient(_qdrantOptions.Host, _qdrantOptions.Port);
        var points = new List<PointStruct>();

        foreach (var issue in issues)
        {
            var issueText = BuildIssueText(issue);
            var embedding = await _ai.GenerateEmbedding(issueText);
            var point = new PointStruct
            {
                Id = Guid.NewGuid(),
                Vectors = new() { Vector = embedding },
                Payload =
                {
                    ["number"] = issue.Number,
                    ["title"] = issue.Title,
                    ["description"] = issue.Description,
                    ["messages"] = JsonSerializer.Serialize(issue.Messages),
                }
            };
            points.Add(point);
        }

        await client.UpsertAsync(CollectionName, points);
    }
}