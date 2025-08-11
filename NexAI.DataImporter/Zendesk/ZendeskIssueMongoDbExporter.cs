using MongoDB.Bson;
using MongoDB.Driver;
using NexAI.Config;
using NexAI.MongoDb;
using NexAI.Zendesk;
using Spectre.Console;

namespace NexAI.DataImporter.Zendesk;

public class ZendeskIssueMongoDbExporter(Options options)
{
    private const string CollectionName = "nexai.zendesk_issues";
    private readonly MongoDbOptions _mongoDbOptions = options.Get<MongoDbOptions>();

    public async Task Export(ZendeskIssue[] zendeskIssues)
    {
        AnsiConsole.MarkupLine("[yellow]Start exporting Zendesk issues into MongoDb...[/]");

        var connectionString = $"mongodb://{Uri.EscapeDataString(_mongoDbOptions.Username)}:{Uri.EscapeDataString(_mongoDbOptions.Password)}@{_mongoDbOptions.Host}:{_mongoDbOptions.Port}";
        var clientSettings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
        var client = new MongoClient(clientSettings);
        var database = client.GetDatabase(_mongoDbOptions.Database);

        var existingCollections = await (await database.ListCollectionNamesAsync()).ToListAsync();
        if (!existingCollections.Contains(CollectionName))
        {
            await database.CreateCollectionAsync(CollectionName);
            var collection = database.GetCollection<BsonDocument>(CollectionName);

            var documents = new List<BsonDocument>();
            foreach (var zendeskIssue in zendeskIssues)
            {
                var issueId = zendeskIssue.Id == Guid.Empty ? Guid.NewGuid() : zendeskIssue.Id;
                var messages = new BsonArray(zendeskIssue.Messages.Select(m => new BsonDocument
                {
                    { "content", m.Content },
                    { "author", m.Author },
                    { "createdAt", m.CreatedAt }
                }));

                var document = new BsonDocument
                {
                    { "_id", issueId },
                    { "number", zendeskIssue.Number },
                    { "title", zendeskIssue.Title },
                    { "description", zendeskIssue.Description },
                    { "messages", messages }
                };

                documents.Add(document);
            }

            if (documents.Count > 0)
            {
                await collection.InsertManyAsync(documents);
            }

            AnsiConsole.MarkupLine("[green]Zendesk issue store initialized.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[green]Zendesk issue already initialized.[/]");
        }
    }
}