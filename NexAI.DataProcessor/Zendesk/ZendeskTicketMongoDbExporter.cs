using MongoDB.Driver;
using NexAI.Config;
using NexAI.MongoDb;
using NexAI.Zendesk;
using NexAI.Zendesk.Messages;
using NexAI.Zendesk.MongoDb;
using Spectre.Console;

namespace NexAI.DataProcessor.Zendesk;

public class ZendeskTicketMongoDbExporter(MongoDbClient mongoDbClient, Options options)
{
    private readonly DataProcessorOptions _dataProcessorOptions = options.Get<DataProcessorOptions>();

    public async Task CreateSchema(CancellationToken cancellationToken)
    {
        var existingCollections = await (await mongoDbClient.Database.ListCollectionNamesAsync(cancellationToken: cancellationToken)).ToListAsync(cancellationToken: cancellationToken);
        if (_dataProcessorOptions.Recreate && existingCollections.Contains(ZendeskTicketMongoDbCollection.Name))
        {
            await mongoDbClient.Database.DropCollectionAsync(ZendeskTicketMongoDbCollection.Name, cancellationToken);
            AnsiConsole.MarkupLine("[red]Deleted collection for Zendesk tickets in MongoDb.[/]");
        }
        if (!existingCollections.Contains(ZendeskTicketMongoDbCollection.Name))
        {
            await mongoDbClient.Database.CreateCollectionAsync(ZendeskTicketMongoDbCollection.Name, cancellationToken: cancellationToken);
            await CreateFullTextIndex(mongoDbClient.GetCollection<ZendeskTicketMongoDbDocument>(ZendeskTicketMongoDbCollection.Name));
            AnsiConsole.MarkupLine("[green]Created schema for Zendesk tickets in MongoDb.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]Collection for Zendesk tickets already exists in MongoDb. Skipping schema creation.[/]");
        }
    }

    public async Task Export(ZendeskTicketImportedEvent zendeskTicketImportedEvent, CancellationToken cancellationToken)
    {
        var zendeskTicket = ZendeskTicket.FromZendeskTicketImportedEvent(zendeskTicketImportedEvent);
        var database = mongoDbClient.Database;
        var collection = database.GetCollection<ZendeskTicketMongoDbDocument>(ZendeskTicketMongoDbCollection.Name);
        var document = await collection.Find(existingZendeskTicket => existingZendeskTicket.Id == zendeskTicket.Id || existingZendeskTicket.ExternalId == zendeskTicket.ExternalId).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (document is not null)
        {
            document.Update(zendeskTicket);
            await collection.ReplaceOneAsync(existingZendeskTicket => existingZendeskTicket.Id == zendeskTicket.Id || existingZendeskTicket.ExternalId == zendeskTicket.ExternalId, document, cancellationToken: cancellationToken);
            AnsiConsole.MarkupLine($"[tan]Successfully updated Zendesk ticket {zendeskTicket.ExternalId} from MongoDb.[/]");
        }
        else
        {
            document = ZendeskTicketMongoDbDocument.Create(zendeskTicket);
            await collection.InsertOneAsync(document, cancellationToken: cancellationToken);
            AnsiConsole.MarkupLine($"[gold3_1]Successfully added Zendesk ticket {zendeskTicket.ExternalId} into MongoDb.[/]");
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