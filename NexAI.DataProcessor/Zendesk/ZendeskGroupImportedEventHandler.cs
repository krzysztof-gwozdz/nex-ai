using NexAI.Zendesk;
using NexAI.Zendesk.Commands;
using NexAI.Zendesk.Messages;
using Spectre.Console;

namespace NexAI.DataProcessor.Zendesk;

public class ZendeskGroupImportedEventHandler(UpsertZendeskGroupCommand upsertZendeskGroupCommand) : IHandleMessages<ZendeskGroupImportedEvent>
{
    public async Task Handle(ZendeskGroupImportedEvent message, IMessageHandlerContext context)
    {
        var zendeskGroup = ZendeskGroup.FromZendeskGroupImportedEvent(message);
        await upsertZendeskGroupCommand.Handle(zendeskGroup);
        AnsiConsole.MarkupLine($"[deepskyblue1]Successfully exported Zendesk group {zendeskGroup.ExternalId} into Neo4j.[/]");
    }
}