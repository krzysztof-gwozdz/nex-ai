using NexAI.Zendesk;
using NexAI.Zendesk.Commands;
using Spectre.Console;

namespace NexAI.DataProcessor.Zendesk;

public class ZendeskUserGroupsNeo4jExporter(UpsertMembersOfRelationCommand upsertMembersOfRelationCommand)
{
    public async Task CreateSchema(CancellationToken cancellationToken)
    {
        AnsiConsole.MarkupLine("[green]Current setup does not require schema creation for Zendesk tickets in Neo4j.[/]");
    }

    public async Task Export(ZendeskUserGroups zendeskUserGroups, CancellationToken cancellationToken)
    {
        await upsertMembersOfRelationCommand.Handle(zendeskUserGroups);
        AnsiConsole.MarkupLine($"[deepskyblue1]Successfully exported Zendesk user groups for user {zendeskUserGroups.UserId} into Neo4j.[/]");
    }
}