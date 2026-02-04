using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NexAI.MongoDb;

namespace NexAI.LLMs.MongoDb;

public class ConversationMongoDbStructure(ILogger<ConversationMongoDbStructure> logger, MongoDbClient mongoDbClient)
{
    public async Task Create(bool recreate, CancellationToken cancellationToken)
    {
        var existingCollections = await (await mongoDbClient.Database.ListCollectionNamesAsync(cancellationToken: cancellationToken)).ToListAsync(cancellationToken: cancellationToken);
        if (recreate && existingCollections.Contains(ConversationMongoDbCollection.Name))
        {
            await mongoDbClient.Database.DropCollectionAsync(ConversationMongoDbCollection.Name, cancellationToken);
            logger.LogInformation("[red]Deleted collection for Conversations in MongoDb.[/]");
        }
        if (!existingCollections.Contains(ConversationMongoDbCollection.Name))
        {
            await mongoDbClient.Database.CreateCollectionAsync(ConversationMongoDbCollection.Name, cancellationToken: cancellationToken);
            logger.LogInformation("[green]Created schema for Conversations in MongoDb.[/]");
        }
        else
        {
            logger.LogInformation("[yellow]Collection for Conversations already exists in MongoDb. Skipping schema creation.[/]");
        }
    }
}