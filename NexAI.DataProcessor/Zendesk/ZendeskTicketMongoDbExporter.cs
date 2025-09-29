using MongoDB.Driver;
using NexAI.Config;
using NexAI.MongoDb;
using NexAI.Zendesk;
using Spectre.Console;

namespace NexAI.DataProcessor.Zendesk;

public class ZendeskTicketMongoDbExporter(MongoDbClient mongoDbClient, Options options)
{
    private readonly DataProcessorOptions _dataProcessorOptions = options.Get<DataProcessorOptions>();

    public async Task CreateSchema(CancellationToken cancellationToken)
    {
        var existingCollections = await (await mongoDbClient.Database.ListCollectionNamesAsync(cancellationToken: cancellationToken)).ToListAsync(cancellationToken: cancellationToken);
        if (_dataProcessorOptions.Recreate && existingCollections.Contains(ZendeskTicketCollections.MongoDbCollectionName))
        {
            await mongoDbClient.Database.DropCollectionAsync(ZendeskTicketCollections.MongoDbCollectionName, cancellationToken);
            AnsiConsole.MarkupLine("[red]Deleted collection for Zendesk tickets in MongoDb.[/]");
        }
        if (!existingCollections.Contains(ZendeskTicketCollections.MongoDbCollectionName))
        {
            await mongoDbClient.Database.CreateCollectionAsync(ZendeskTicketCollections.MongoDbCollectionName, cancellationToken: cancellationToken);
            await CreateFullTextIndex(mongoDbClient.GetCollection<ZendeskTicketMongoDbDocument>(ZendeskTicketCollections.MongoDbCollectionName));
            AnsiConsole.MarkupLine("[green]Created schema for Zendesk tickets in MongoDb.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]Collection for Zendesk tickets already exists in MongoDb. Skipping schema creation.[/]");
        }
    }

    public async Task Export(ZendeskTicket zendeskTicket, CancellationToken cancellationToken)
    {
        var database = mongoDbClient.Database;
        var collection = database.GetCollection<ZendeskTicketMongoDbDocument>(ZendeskTicketCollections.MongoDbCollectionName);
        var document = await collection.Find(existingZendeskTicket => existingZendeskTicket.Id == zendeskTicket.Id || existingZendeskTicket.ExternalId == zendeskTicket.Id).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (document is not null)
        {
            document.Update(zendeskTicket);
            await collection.ReplaceOneAsync(existingZendeskTicket => existingZendeskTicket.Id == zendeskTicket.Id || existingZendeskTicket.ExternalId == zendeskTicket.Id, document, cancellationToken: cancellationToken);
            AnsiConsole.MarkupLine($"[green]Successfully updated Zendesk ticket {zendeskTicket.ExternalId} from MongoDb.[/]");
        }
        else
        {
            document = ZendeskTicketMongoDbDocument.Create(zendeskTicket);
            await collection.InsertOneAsync(document, cancellationToken: cancellationToken);
            AnsiConsole.MarkupLine($"[green]Successfully added Zendesk ticket {zendeskTicket.ExternalId} into MongoDb.[/]");
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