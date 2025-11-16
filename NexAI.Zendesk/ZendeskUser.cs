using NexAI.Zendesk.Messages;

namespace NexAI.Zendesk;

public record ZendeskUser(ZendeskUserId Id, string ExternalId, string Name, string Email)
{
    public static ZendeskUser Create(string externalId, string name, string email) =>
        new(ZendeskUserId.New(), externalId, name, email);
    
    public static ZendeskUser FromZendeskUserImportedEvent(ZendeskUserImportedEvent zendeskUserImportedEvent) =>
        new(
            new(zendeskUserImportedEvent.Id),
            zendeskUserImportedEvent.ExternalId,
            zendeskUserImportedEvent.Name,
            zendeskUserImportedEvent.Email
        );

    public ZendeskUserImportedEvent ToZendeskUserImportedEvent() =>
        new(Id.Value, ExternalId, Name, Email);
}