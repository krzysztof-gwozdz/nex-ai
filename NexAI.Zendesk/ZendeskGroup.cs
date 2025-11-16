using NexAI.Zendesk.Messages;

namespace NexAI.Zendesk;

public record ZendeskGroup(ZendeskGroupId Id, string ExternalId, string Name)
{
    public static ZendeskGroup Create(string externalId, string name) =>
        new(ZendeskGroupId.New(), externalId, name);

    public static ZendeskGroup FromZendeskGroupImportedEvent(ZendeskGroupImportedEvent zendeskGroupImportedEvent) =>
        new(
            new(zendeskGroupImportedEvent.Id),
            zendeskGroupImportedEvent.ExternalId,
            zendeskGroupImportedEvent.Name
        );

    public ZendeskGroupImportedEvent ToZendeskGroupImportedEvent() =>
        new(Id.Value, ExternalId, Name);
}