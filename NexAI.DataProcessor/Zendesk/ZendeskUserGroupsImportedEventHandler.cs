using NexAI.Zendesk;
using NexAI.Zendesk.Commands;
using NexAI.Zendesk.Messages;
using Spectre.Console;

namespace NexAI.DataProcessor.Zendesk;

public class ZendeskUserGroupsImportedEventHandler(UpsertZendeskMembersOfRelationshipCommand upsertZendeskMembersOfRelationshipCommand) : IHandleMessages<ZendeskUserGroupsImportedEvent>
{ 
    public async Task Handle(ZendeskUserGroupsImportedEvent message, IMessageHandlerContext context)
    {
        var zendeskUserGroups = ZendeskUserGroups.FromZendeskUserGroupsImportedEvent(message);
        await upsertZendeskMembersOfRelationshipCommand.Handle(zendeskUserGroups);
        AnsiConsole.MarkupLine($"[deepskyblue1]Successfully exported Zendesk user groups for user {zendeskUserGroups.UserId} into Neo4j.[/]");
    }
}