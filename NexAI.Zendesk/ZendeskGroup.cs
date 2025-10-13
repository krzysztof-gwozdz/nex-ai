namespace NexAI.Zendesk;

public record ZendeskGroup(ZendeskGroupId Id, string ExternalId, string Name)
{
    public static ZendeskGroup Create(string externalId, string name) =>
        new(ZendeskGroupId.New(), externalId, name);
}