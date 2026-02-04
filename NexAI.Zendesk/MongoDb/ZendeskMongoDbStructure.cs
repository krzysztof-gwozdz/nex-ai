using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NexAI.MongoDb;

namespace NexAI.Zendesk.MongoDb;

public class ZendeskMongoDbStructure(ILogger<ZendeskMongoDbStructure> logger, MongoDbClient mongoDbClient)
{
    public async Task Create(bool recreate, CancellationToken cancellationToken)
    {
        var existingCollections = await (await mongoDbClient.Database.ListCollectionNamesAsync(cancellationToken: cancellationToken)).ToListAsync(cancellationToken: cancellationToken);
        if (recreate && existingCollections.Contains(ZendeskTicketMongoDbCollection.Name))
        {
            await mongoDbClient.Database.DropCollectionAsync(ZendeskTicketMongoDbCollection.Name, cancellationToken);
            logger.LogInformation("[red]Deleted collection for Zendesk tickets in MongoDb.[/]");
        }
        if (!existingCollections.Contains(ZendeskTicketMongoDbCollection.Name))
        {
            await mongoDbClient.Database.CreateCollectionAsync(ZendeskTicketMongoDbCollection.Name, cancellationToken: cancellationToken);
            await CreateFullTextIndex(mongoDbClient.Database.GetCollection<ZendeskTicketMongoDbDocument>(ZendeskTicketMongoDbCollection.Name));
            logger.LogInformation("[green]Created schema for Zendesk tickets in MongoDb.[/]");
        }
        else
        {
            logger.LogInformation("[yellow]Collection for Zendesk tickets already exists in MongoDb. Skipping schema creation.[/]");
        }
    }

    private static async Task CreateFullTextIndex(IMongoCollection<ZendeskTicketMongoDbDocument> collection)
    {
        var indexKeys = Builders<ZendeskTicketMongoDbDocument>.IndexKeys
            .Text(zendeskTicket => zendeskTicket.Title)
            .Text(zendeskTicket => zendeskTicket.Description)
            .Text(zendeskTicket => zendeskTicket.Messages.Select(message => message.Content));
        var indexModel = new CreateIndexModel<ZendeskTicketMongoDbDocument>(indexKeys);
        await collection.Indexes.CreateOneAsync(indexModel);
    }
}