using MongoDB.Driver;
using NexAI.MongoDb;
using NexAI.Zendesk.MongoDb;

namespace NexAI.Zendesk.Queries;

public class FindZendeskTicketsThatContainPhraseQuery(MongoDbClient mongoDbClient)
{
    public async Task<SearchResult[]> Handle(string phrase, int limit, CancellationToken cancellationToken)
    {
        var collection = mongoDbClient.GetCollection<ZendeskTicketMongoDbDocument>(ZendeskTicketMongoDbCollection.Name);
        var filter = Builders<ZendeskTicketMongoDbDocument>.Filter.Text(phrase);

        var results = await collection
            .Find(filter)
            .Limit(limit)
            .Project<ZendeskTicketMongoDbDocument>(Builders<ZendeskTicketMongoDbDocument>.Projection.MetaTextScore("score"))
            .Sort(Builders<ZendeskTicketMongoDbDocument>.Sort.MetaTextScore("score"))
            .ToListAsync(cancellationToken: cancellationToken);
        
        var maxScore = results.Max(document => document.Score);
        return results.Select(document => SearchResult.FullTextSearchResult(document.ToZendeskTicket(), document.Score/maxScore)).ToArray();
    }
}