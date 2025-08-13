using MongoDB.Driver;
using NexAI.Config;
using NexAI.MongoDb;
using NexAI.Zendesk;
using Spectre.Console;

namespace NexAI.DataImporter.Zendesk;

public class ZendeskIssueMongoDbExporter(Options options)
{
    private readonly MongoDbOptions _mongoDbOptions = options.Get<MongoDbOptions>();

    public async Task Export(ZendeskIssue[] zendeskIssues)
    {
        AnsiConsole.MarkupLine("[yellow]Start exporting Zendesk issues into MongoDb...[/]");
        var clientSettings = MongoClientSettings.FromUrl(new(_mongoDbOptions.ConnectionString));
        var client = new MongoClient(clientSettings);
        var database = client.GetDatabase(_mongoDbOptions.Database);
        var existingCollections = await (await database.ListCollectionNamesAsync()).ToListAsync();
        if (!existingCollections.Contains(ZendeskIssueCollections.MongoDbCollectionName))
        {
            await database.CreateCollectionAsync(ZendeskIssueCollections.MongoDbCollectionName);
            var collection = database.GetCollection<ZendeskIssueMongoDbDocument>(ZendeskIssueCollections.MongoDbCollectionName);
            await InsertData(zendeskIssues, collection);
            AnsiConsole.MarkupLine("[green]Zendesk issue store initialized.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[green]Zendesk issue already initialized.[/]");
        }
    }

    private static async Task InsertData(ZendeskIssue[] zendeskIssues, IMongoCollection<ZendeskIssueMongoDbDocument> collection)
    {
        var documents = new List<ZendeskIssueMongoDbDocument>();
        foreach (var zendeskIssue in zendeskIssues)
        {
            var document = new ZendeskIssueMongoDbDocument
            {
                Id = zendeskIssue.Id,
                Number = zendeskIssue.Number,
                Title = zendeskIssue.Title,
                Description = zendeskIssue.Description,
                Messages = zendeskIssue.Messages.Select(m => new ZendeskIssueMongoDbDocument.MessageDocument
                {
                    Content = m.Content,
                    Author = m.Author,
                    CreatedAt = m.CreatedAt
                }).ToArray()
            };
            documents.Add(document);
        }

        if (documents.Count > 0)
        {
            await collection.InsertManyAsync(documents);
        }
    }
}