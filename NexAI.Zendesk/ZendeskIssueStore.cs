using System.Text.Json;
using NexAI.Config;
using NexAI.OpenAI;
using NexAI.Qdrant;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace NexAI.Zendesk;

public class ZendeskIssueStore(Options options)
{
    private const string CollectionName = "nexai.zendesk_issues";
    private readonly QdrantOptions _qdrantOptions = options.Get<QdrantOptions>();
    private readonly TextEmbedder _textEmbedder = new(options);
    
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
            Guid.Parse(point.Id.Uuid),
            point.Payload["number"].StringValue,
            point.Payload["title"].StringValue,
            point.Payload["description"].StringValue,
            JsonSerializer.Deserialize<ZendeskIssue.ZendeskIssueMessage[]>(point.Payload["messages"].StringValue) ?? []
        );
    }

    public async Task<List<SimilarIssue>> FindSimilarIssuesByNumber(string issueId, ulong limit)
    {
        var zendeskIssue = await GetIssueByNumber(issueId);
        if (zendeskIssue == null)
            return [];

        return (await FindSimilarIssuesByPhrase(zendeskIssue.ToString(), limit + 1))
            .Where(issue => issue.Number != issueId)
            .ToList();
    }

    public async Task<List<SimilarIssue>> FindSimilarIssuesByPhrase(string text, ulong limit)
    {
        using var client = new QdrantClient(_qdrantOptions.Host, _qdrantOptions.Port);
        var embedding = await _textEmbedder.GenerateEmbedding(text);
        var response = await client.SearchAsync(CollectionName, embedding, limit: limit);

        return response
            .Select(point => new SimilarIssue(
                point.Payload["number"].StringValue,
                point.Payload["title"].StringValue,
                point.Score)
            )
            .ToList();
    }
}