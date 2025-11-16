namespace NexAI.Zendesk.Messages;

public record ZendeskUserImportedEvent(Guid Id, string ExternalId, string Name, string Email);