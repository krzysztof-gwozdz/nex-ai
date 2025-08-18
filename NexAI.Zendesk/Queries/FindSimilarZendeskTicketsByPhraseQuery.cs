using NexAI.Config;
using NexAI.LLMs;
using NexAI.Qdrant;
using Qdrant.Client;

namespace NexAI.Zendesk.Queries;

public class FindSimilarZendeskTicketsByPhraseQuery(Options options)
{
    private readonly QdrantOptions _qdrantOptions = options.Get<QdrantOptions>();
    private readonly OpenAITextEmbedder _textEmbedder = new(options);
    
    public async Task<List<SimilarTicket>> Handle(string phrase, int limit)
    {
        using var client = new QdrantClient(_qdrantOptions.Host, _qdrantOptions.Port);
        var embedding = await _textEmbedder.GenerateEmbedding(phrase);
        var response = await client.SearchAsync(ZendeskTicketCollections.QdrantCollectionName, embedding, limit: (ulong)limit);
        return response
            .Select(point => new SimilarTicket(point.Payload["number"].StringValue, point.Score))
            .ToList();
    }
}