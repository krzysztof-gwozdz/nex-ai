using MongoDB.Driver;
using NexAI.MongoDb;

namespace NexAI.Zendesk.Queries;

public class FindZendeskTicketsThatContainPhraseQuery(MongoDbClient mongoDbClient)
{
    public async Task<SearchResult[]> Handle(string phrase, int limit, CancellationToken cancellationToken)
    {
        var collection = mongoDbClient.GetCollection<ZendeskTicketMongoDbDocument>(ZendeskTicketCollections.MongoDbCollectionName);
        var filter = Builders<ZendeskTicketMongoDbDocument>.Filter.Text(phrase);

        var projection = Builders<ZendeskTicketMongoDbDocument>.Projection
            .MetaTextScore("score");

        var sort = Builders<ZendeskTicketMongoDbDocument>.Sort
            .MetaTextScore("score");

        var results = await collection
            .Find(filter)
            .Limit(limit)
            .Project<ZendeskTicketMongoDbDocument>(projection)
            .Sort(sort)
            .ToListAsync(cancellationToken: cancellationToken);
        
        return results.Select(document => SearchResult.FullTextSearchResult(document.ToZendeskTicket(), document.Score)).ToArray();
    }
}