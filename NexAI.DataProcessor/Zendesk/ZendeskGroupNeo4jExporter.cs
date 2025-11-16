using NexAI.Zendesk;
using NexAI.Zendesk.Commands;
using NexAI.Zendesk.Messages;
using Spectre.Console;

namespace NexAI.DataProcessor.Zendesk;

public class ZendeskGroupNeo4jExporter(UpsertZendeskGroupCommand upsertZendeskGroupCommand)
{
    public Task CreateSchema(CancellationToken cancellationToken)
    {
        AnsiConsole.MarkupLine("[green]Current setup does not require schema creation for Zendesk tickets in Neo4j.[/]");
        return Task.CompletedTask;
    }

    public async Task Export(ZendeskGroupImportedEvent zendeskGroupImportedEvent, CancellationToken cancellationToken)
    {
        var zendeskGroup = ZendeskGroup.FromZendeskGroupImportedEvent(zendeskGroupImportedEvent);
        await upsertZendeskGroupCommand.Handle(zendeskGroup);
        AnsiConsole.MarkupLine($"[deepskyblue1]Successfully exported Zendesk group {zendeskGroup.ExternalId} into Neo4j.[/]");
    }
}