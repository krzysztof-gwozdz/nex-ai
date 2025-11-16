using NexAI.Zendesk;
using NexAI.Zendesk.Commands;
using NexAI.Zendesk.Messages;
using Spectre.Console;

namespace NexAI.DataProcessor.Zendesk;

public class ZendeskUserNeo4jExporter(UpsertZendeskUserCommand upsertZendeskUserCommand)
{
    public Task CreateSchema(CancellationToken cancellationToken)
    {
        AnsiConsole.MarkupLine("[green]Current setup does not require schema creation for Zendesk tickets in Neo4j.[/]");
        return Task.CompletedTask;
    }

    public async Task Export(ZendeskUserImportedEvent zendeskUserImportedEvent, CancellationToken cancellationToken)
    {
        var zendeskUser = ZendeskUser.FromZendeskUserImportedEvent(zendeskUserImportedEvent);
        await upsertZendeskUserCommand.Handle(zendeskUser);
        AnsiConsole.MarkupLine($"[deepskyblue1]Successfully exported Zendesk user {zendeskUser.Id} into Neo4j.[/]");
    }
}