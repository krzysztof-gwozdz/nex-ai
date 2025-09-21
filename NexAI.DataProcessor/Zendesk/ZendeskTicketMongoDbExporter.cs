using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using NexAI.Config;
using NexAI.MongoDb;
using NexAI.Zendesk;
using Spectre.Console;

namespace NexAI.DataProcessor.Zendesk;

public class ZendeskTicketMongoDbExporter(Options options)
{
    private readonly DataProcessorOptions _dataProcessorOptions = options.Get<DataProcessorOptions>();
    private readonly MongoDbOptions _mongoDbOptions = options.Get<MongoDbOptions>();

    public async Task CreateSchema()
    {
        RegisterGuidSerializer();
        var clientSettings = MongoClientSettings.FromUrl(new(_mongoDbOptions.ConnectionString));
        var client = new MongoClient(clientSettings);
        var database = client.GetDatabase(_mongoDbOptions.Database);
        var existingCollections = await (await database.ListCollectionNamesAsync()).ToListAsync();
        if (_dataProcessorOptions.Recreate && existingCollections.Contains(ZendeskTicketCollections.MongoDbCollectionName))
        {
            await database.DropCollectionAsync(ZendeskTicketCollections.MongoDbCollectionName);
            AnsiConsole.MarkupLine("[red]Deleted collection for Zendesk tickets in MongoDb.[/]");
        }
        if (!existingCollections.Contains(ZendeskTicketCollections.MongoDbCollectionName))
        {
            await database.CreateCollectionAsync(ZendeskTicketCollections.MongoDbCollectionName);
            await CreateFullTextIndex(database.GetCollection<ZendeskTicketMongoDbDocument>(ZendeskTicketCollections.MongoDbCollectionName));
            AnsiConsole.MarkupLine("[green]Created schema for Zendesk tickets in MongoDb.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]Collection for Zendesk tickets already exists in MongoDb. Skipping schema creation.[/]");
        }
    }

    public async Task Export(ZendeskTicket zendeskTicket)
    {
        var clientSettings = MongoClientSettings.FromUrl(new(_mongoDbOptions.ConnectionString));
        var client = new MongoClient(clientSettings);
        var database = client.GetDatabase(_mongoDbOptions.Database);
        var collection = database.GetCollection<ZendeskTicketMongoDbDocument>(ZendeskTicketCollections.MongoDbCollectionName);
        var document = ZendeskTicketMongoDbDocument.Create(zendeskTicket);
        await collection.InsertOneAsync(document);
        AnsiConsole.MarkupLine("[green]Successfully exported Zendesk tickets into MongoDb.[/]");
    }

    private static void RegisterGuidSerializer()
    {
        try
        {
            BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        }
        catch
        {
            // ignored
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