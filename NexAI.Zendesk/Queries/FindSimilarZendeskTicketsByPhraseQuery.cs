using NexAI.LLMs.Common;
using NexAI.Qdrant;
using NexAI.Zendesk.QdrantDb;
using Qdrant.Client.Grpc;

namespace NexAI.Zendesk.Queries;

public class FindSimilarZendeskTicketsByPhraseQuery(
    QdrantDbClient qdrantDbClient,
    TextEmbedder textEmbedder,
    GetZendeskTicketsByIdsQuery getZendeskTicketsByIdsQuery)
{
    public async Task<SearchResult[]> Handle(string phrase, int limit, CancellationToken cancellationToken)
    {
        var embedding = await textEmbedder.GenerateEmbedding(phrase, cancellationToken);

        var searchResults = await SearchForPhrase(embedding, "checkoutprocessing", limit, cancellationToken);

        var typeWeights = new Dictionary<string, float>
        {
            ["ticket"] = 0.5f,
            ["title_and_description"] = 0.3f,
            ["message"] = 0.2f
        };

        var weightSearchResults = searchResults
            .GroupBy(result => result.TicketId)
            .Select(groupSearchResults =>
            {
                var weightedScore = groupSearchResults.Sum(result => result.Score * typeWeights[result.Type]);
                var description = $"Tickets: {groupSearchResults.Count(x => x.Type == "ticket")}"
                                  + $" | Titles/Descriptions: {groupSearchResults.Count(x => x.Type == "title_and_description")}"
                                  + $" | Message: {groupSearchResults.Count(x => x.Type == "message")}";
                return (
                    TicketId: groupSearchResults.Key,
                    Score: weightedScore,
                    Description: description
                );
            })
            .OrderByDescending(x => x.Score)
            .ToArray();

        var maxScore = weightSearchResults.Max(x => x.Score);
        var zendeskTickets = await getZendeskTicketsByIdsQuery.Handle(weightSearchResults.Select(searchResult => searchResult.TicketId).ToArray(), cancellationToken);
        return zendeskTickets
            .Select(zendeskTicket =>
            {
                var weightSearchResult = weightSearchResults.First(result => result.TicketId == zendeskTicket.Id);
                return SearchResult.EmbeddingBasedSearchResult(zendeskTicket, weightSearchResult.Score / maxScore, weightSearchResult.Description);
            })
            .OrderByDescending(x => x.Score)
            .Take(limit)
            .ToArray();
    }

    private async Task<(string Type, Guid TicketId, string ExternalId, string Level3Team, float Score)[]> SearchForPhrase
        (ReadOnlyMemory<float> embedding, string? level3TeamFilter, int limit, CancellationToken cancellationToken)
    {
        var level3TeamCondition = level3TeamFilter is null
            ? null
            : Conditions.MatchKeyword("level3_team", level3TeamFilter);

        var ticketTypeFilter = Conditions.MatchKeyword("type", "ticket");
        Filter ticketFilter = level3TeamCondition is null
            ? new() { Must = { ticketTypeFilter } }
            : new() { Must = { ticketTypeFilter, level3TeamCondition } };

        var titleAndDescriptionTypeFilter = Conditions.MatchKeyword("type", "title_and_description");
        Filter titleAndDescriptionFilter = level3TeamCondition is null
            ? new() { Must = { titleAndDescriptionTypeFilter } }
            : new() { Must = { titleAndDescriptionTypeFilter, level3TeamCondition } };

        var messageTypeFilter = Conditions.MatchKeyword("type", "message");
        Filter messageFilter = level3TeamCondition is null
            ? new() { Must = { messageTypeFilter } }
            : new() { Must = { messageTypeFilter, level3TeamCondition } };

        var tasks = new[]
        {
            qdrantDbClient.SearchAsync(
                ZendeskTicketQdrantCollection.Name,
                embedding,
                filter: ticketFilter,
                limit: (ulong)limit,
                cancellationToken: cancellationToken),
            qdrantDbClient.SearchAsync(
                ZendeskTicketQdrantCollection.Name,
                embedding,
                filter: titleAndDescriptionFilter,
                limit: (ulong)limit,
                cancellationToken: cancellationToken
            ),
            qdrantDbClient.SearchAsync(
                ZendeskTicketQdrantCollection.Name,
                embedding,
                filter: messageFilter,
                limit: (ulong)limit * 5,
                cancellationToken: cancellationToken
            )
        };

        var results = (await Task.WhenAll(tasks))
            .SelectMany(x => x)
            .Select(point => (
                Type: point.Payload["type"].StringValue,
                TicketId: Guid.Parse(point.Payload["ticket_id"].StringValue),
                ExternalId: point.Payload["external_id"].StringValue,
                Level3Team: point.Payload["level3_team"].StringValue,
                Score: point.Score
            ))
            .ToArray();
        return results;
    }
}