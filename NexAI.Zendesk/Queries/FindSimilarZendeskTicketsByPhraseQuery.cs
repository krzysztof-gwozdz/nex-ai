using NexAI.Config;
using NexAI.LLMs;
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
            .Select(result => (Number: result.Payload["number"].StringValue, result.Score)).ToArray();
        var zendeskTickets = await new GetZendeskTicketsByNumbersQuery(options).Handle(searchResult.Select(result => result.Number).ToArray());
        return zendeskTickets
            .Select(zendeskTicket => SearchResult.EmbeddingBasedSearchResult(zendeskTicket, searchResult.First(result => result.Number == zendeskTicket.Number).Score))
            .ToArray();
    }
}