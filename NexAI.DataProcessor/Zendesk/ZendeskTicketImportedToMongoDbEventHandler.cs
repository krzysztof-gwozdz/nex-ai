using MongoDB.Driver;
using NexAI.MongoDb;
using NexAI.Zendesk;
using NexAI.Zendesk.Messages;
using NexAI.Zendesk.MongoDb;
using Spectre.Console;

namespace NexAI.DataProcessor.Zendesk;

public class ZendeskTicketImportedToMongoDbEventHandler(MongoDbClient mongoDbClient) : IHandleMessages<ZendeskTicketImportedEvent>
{
    public async Task Handle(ZendeskTicketImportedEvent message, IMessageHandlerContext context)
    {
        var zendeskTicket = ZendeskTicket.FromZendeskTicketImportedEvent(message);
        var database = mongoDbClient.Database;
        var collection = database.GetCollection<ZendeskTicketMongoDbDocument>(ZendeskTicketMongoDbCollection.Name);
        var document = await collection.Find(existingZendeskTicket => existingZendeskTicket.Id == zendeskTicket.Id || existingZendeskTicket.ExternalId == zendeskTicket.ExternalId).FirstOrDefaultAsync(cancellationToken: context.CancellationToken);
        if (document is not null)
        {
            document.Update(zendeskTicket);
            await collection.ReplaceOneAsync(existingZendeskTicket => existingZendeskTicket.Id == zendeskTicket.Id || existingZendeskTicket.ExternalId == zendeskTicket.ExternalId, document, cancellationToken: context.CancellationToken);
            AnsiConsole.MarkupLine($"[tan]Successfully updated Zendesk ticket {zendeskTicket.ExternalId} from MongoDb.[/]");
        }
        else
        {
            document = ZendeskTicketMongoDbDocument.Create(zendeskTicket);
            await collection.InsertOneAsync(document, cancellationToken: context.CancellationToken);
            AnsiConsole.MarkupLine($"[gold3_1]Successfully added Zendesk ticket {zendeskTicket.ExternalId} into MongoDb.[/]");
        }
    }
}