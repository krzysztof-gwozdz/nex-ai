namespace NexAI.Zendesk;

public record ZendeskUser(ZendeskUserId Id, string ExternalId, string Name)
{
    public static ZendeskUser Create(string externalId, string name) =>
        new(ZendeskUserId.New(), externalId, name);
}