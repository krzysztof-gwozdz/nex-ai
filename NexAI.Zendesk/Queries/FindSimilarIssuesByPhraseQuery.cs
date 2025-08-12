using NexAI.Config;
using NexAI.OpenAI;
using NexAI.Qdrant;
using Qdrant.Client;

namespace NexAI.Zendesk.Queries;

public class FindSimilarIssuesByPhraseQuery(Options options)
{
    private readonly QdrantOptions _qdrantOptions = options.Get<QdrantOptions>();
    private readonly TextEmbedder _textEmbedder = new(options);
    
    public async Task<List<SimilarIssue>> Handle(string text, ulong limit)
    {
        using var client = new QdrantClient(_qdrantOptions.Host, _qdrantOptions.Port);
        var embedding = await _textEmbedder.GenerateEmbedding(text);
        var response = await client.SearchAsync(ZendeskIssueCollections.QdrantCollectionName, embedding, limit: limit);

        return response
            .Select(point => new SimilarIssue(point.Payload["number"].StringValue, point.Score))
            .ToList();
    }
}