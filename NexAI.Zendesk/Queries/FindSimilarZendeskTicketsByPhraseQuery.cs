using NexAI.LLMs.Common;
using NexAI.Qdrant;

namespace NexAI.Zendesk.Queries;

public class FindSimilarZendeskTicketsByPhraseQuery(
    QdrantDbClient qdrantDbClient,
    TextEmbedder textEmbedder,
    GetZendeskTicketsByIdsQuery getZendeskTicketsByIdsQuery)
{
    public async Task<SearchResult[]> Handle(string phrase, int limit)
    {
        var embedding = await textEmbedder.GenerateEmbedding(phrase);
        var searchResult = (await qdrantDbClient.SearchAsync(ZendeskTicketCollections.QdrantCollectionName, embedding, limit: (ulong)limit))
            .Select(point => (Id: Guid.Parse(point.Payload["ticket_id"].StringValue), point.Score)).ToArray();
        var zendeskTickets = await getZendeskTicketsByIdsQuery
            .Handle(searchResult.Select(result => result.Id).ToArray());
        return zendeskTickets
            .Select(zendeskTicket => SearchResult.EmbeddingBasedSearchResult(zendeskTicket, searchResult.First(result => result.Id == zendeskTicket.Id).Score))
            .ToArray();
    }
}