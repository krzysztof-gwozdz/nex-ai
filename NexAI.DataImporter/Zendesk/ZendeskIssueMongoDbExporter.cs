using MongoDB.Bson;
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
            var collection = database.GetCollection<BsonDocument>(ZendeskIssueCollections.MongoDbCollectionName);

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
                    { "_id", new BsonBinaryData(issueId, GuidRepresentation.CSharpLegacy) },
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