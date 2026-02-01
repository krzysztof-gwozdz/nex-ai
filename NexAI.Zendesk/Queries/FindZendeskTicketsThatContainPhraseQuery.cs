using MongoDB.Driver;
using NexAI.Zendesk.MongoDb;

namespace NexAI.Zendesk.Queries;

public class FindZendeskTicketsThatContainPhraseQuery(ZendeskTicketMongoDbCollection zendeskTicketMongoDbCollection)
{
    public async Task<SearchResult[]> Handle(string phrase, int limit, CancellationToken cancellationToken)
    {
        var filter = Builders<ZendeskTicketMongoDbDocument>.Filter.Text(phrase);

        var results = await zendeskTicketMongoDbCollection.Collection
            .Find(filter)
            .Limit(limit)
            .Project<ZendeskTicketMongoDbDocument>(Builders<ZendeskTicketMongoDbDocument>.Projection.MetaTextScore("score"))
            .Sort(Builders<ZendeskTicketMongoDbDocument>.Sort.MetaTextScore("score"))
            .ToListAsync(cancellationToken: cancellationToken);
        
        var maxScore = results.Max(document => document.Score);
        return results.Select(document => SearchResult.FullTextSearchResult(document.ToZendeskTicket(), document.Score/maxScore)).ToArray();
    }
}