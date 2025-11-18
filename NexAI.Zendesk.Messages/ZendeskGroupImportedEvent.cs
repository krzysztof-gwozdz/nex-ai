namespace NexAI.Zendesk.Messages;

public record ZendeskGroupImportedEvent(Guid Id, string ExternalId, string Name) : IEvent;