namespace NexAI.Zendesk;

public record ZendeskUser(ZendeskUserId Id, string ExternalId, string Name, string Email)
{
    public static ZendeskUser Create(string externalId, string name, string email) =>
        new(ZendeskUserId.New(), externalId, name, email);
}