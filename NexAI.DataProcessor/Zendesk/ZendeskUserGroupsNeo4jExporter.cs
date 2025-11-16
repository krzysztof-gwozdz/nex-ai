using NexAI.Zendesk;
using NexAI.Zendesk.Commands;
using NexAI.Zendesk.Messages;
using Spectre.Console;

namespace NexAI.DataProcessor.Zendesk;

public class ZendeskUserGroupsNeo4jExporter(UpsertZendeskMembersOfRelationshipCommand upsertZendeskMembersOfRelationshipCommand)
{
    public Task CreateSchema(CancellationToken cancellationToken)
    {
        AnsiConsole.MarkupLine("[green]Current setup does not require schema creation for Zendesk tickets in Neo4j.[/]");
        return Task.CompletedTask;
    }

    public async Task Export(ZendeskUserGroupsImportedEvent zendeskUserGroupsImportedEvent, CancellationToken cancellationToken)
    {
        var zendeskUserGroups = ZendeskUserGroups.FromZendeskUserGroupsImportedEvent(zendeskUserGroupsImportedEvent);
        await upsertZendeskMembersOfRelationshipCommand.Handle(zendeskUserGroups);
        AnsiConsole.MarkupLine($"[deepskyblue1]Successfully exported Zendesk user groups for user {zendeskUserGroups.UserId} into Neo4j.[/]");
    }
}