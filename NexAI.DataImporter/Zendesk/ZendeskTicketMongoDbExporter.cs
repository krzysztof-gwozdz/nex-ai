using MongoDB.Driver;
using NexAI.Config;
using NexAI.MongoDb;
using NexAI.Zendesk;
using Spectre.Console;

namespace NexAI.DataImporter.Zendesk;

public class ZendeskTicketMongoDbExporter(Options options)
{
    private readonly MongoDbOptions _mongoDbOptions = options.Get<MongoDbOptions>();

    public async Task Export(ZendeskTicket[] zendeskTickets)
    {
        AnsiConsole.MarkupLine("[yellow]Start exporting Zendesk tickets into MongoDb...[/]");
        var clientSettings = MongoClientSettings.FromUrl(new(_mongoDbOptions.ConnectionString));
        var client = new MongoClient(clientSettings);
        var database = client.GetDatabase(_mongoDbOptions.Database);
        await CreateSchema(database);
        await InsertData(zendeskTickets, database);
    }

    private static async Task CreateSchema(IMongoDatabase database)
    {
        var existingCollections = await (await database.ListCollectionNamesAsync()).ToListAsync();
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

    private static async Task CreateFullTextIndex(IMongoCollection<ZendeskTicketMongoDbDocument> collection)
    {
        var indexKeys = Builders<ZendeskTicketMongoDbDocument>.IndexKeys
            .Text(zendeskTicket => zendeskTicket.Title)
            .Text(zendeskTicket => zendeskTicket.Description)
            .Text(zendeskTicket => zendeskTicket.Messages.Select(message => message.Content));
        var indexModel = new CreateIndexModel<ZendeskTicketMongoDbDocument>(indexKeys);
        await collection.Indexes.CreateOneAsync(indexModel);
    }

    private static async Task InsertData(ZendeskTicket[] zendeskTickets, IMongoDatabase database)
    {
        var collection = database.GetCollection<ZendeskTicketMongoDbDocument>(ZendeskTicketCollections.MongoDbCollectionName);
        if (await collection.EstimatedDocumentCountAsync() == 0)
        {
            var documents = new List<ZendeskTicketMongoDbDocument>();
            foreach (var zendeskTicket in zendeskTickets)
            {
                var document = new ZendeskTicketMongoDbDocument
                {
                    Id = zendeskTicket.Id,
                    Number = zendeskTicket.Number,
                    Title = zendeskTicket.Title,
                    Description = zendeskTicket.Description,
                    Messages = zendeskTicket.Messages.Select(m => new ZendeskTicketMongoDbDocument.MessageDocument
                    {
                        Content = m.Content,
                        Author = m.Author,
                        CreatedAt = m.CreatedAt
                    }).ToArray()
                };
                documents.Add(document);
            }
            await collection.InsertManyAsync(documents);
            AnsiConsole.MarkupLine("[green]Successfully exported Zendesk tickets into Mongo.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]Zendesk tickets already exported into Mongo. Skipping export.[/]");
        }
    }
}