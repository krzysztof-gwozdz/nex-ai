using NexAI.Zendesk;
using NexAI.Zendesk.Commands;
using NexAI.Zendesk.Messages;
using Spectre.Console;

namespace NexAI.DataProcessor.Zendesk;

public class ZendeskUserImportedEventHandler(UpsertZendeskUserCommand upsertZendeskUserCommand) : IHandleMessages<ZendeskUserImportedEvent>
{
    public async Task Handle(ZendeskUserImportedEvent message, IMessageHandlerContext context)
    {
        var zendeskUser = ZendeskUser.FromZendeskUserImportedEvent(message);
        await upsertZendeskUserCommand.Handle(zendeskUser);
        AnsiConsole.MarkupLine($"[deepskyblue1]Successfully exported Zendesk user {zendeskUser.Id} into Neo4j.[/]");
    }
}