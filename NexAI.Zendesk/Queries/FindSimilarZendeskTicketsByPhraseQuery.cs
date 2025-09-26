using NexAI.Config;
using NexAI.LLMs.Common;
using NexAI.Qdrant;
using Qdrant.Client;

namespace NexAI.Zendesk.Queries;

public class FindSimilarZendeskTicketsByPhraseQuery(Options options)
{
    private readonly QdrantOptions _qdrantOptions = options.Get<QdrantOptions>();
    private readonly TextEmbedder _textEmbedder = TextEmbedder.GetInstance(options);

    public async Task<SearchResult[]> Handle(string phrase, int limit)
    {
        using var client = new QdrantClient(_qdrantOptions.Host, _qdrantOptions.Port);
        var embedding = await _textEmbedder.GenerateEmbedding(phrase);
        var searchResult = (await client.SearchAsync(ZendeskTicketCollections.QdrantCollectionName, embedding, limit: (ulong)limit))
            .Select(point => (Id: Guid.Parse(point.Id.Uuid), point.Score)).ToArray();
        var zendeskTickets = await new GetZendeskTicketsByIdsQuery(options)
            .Handle(searchResult.Select(result => result.Id).ToArray());
        return zendeskTickets
            .Select(zendeskTicket => SearchResult.EmbeddingBasedSearchResult(zendeskTicket, searchResult.First(result => result.Id == zendeskTicket.Id).Score))
            .ToArray();
    }
}