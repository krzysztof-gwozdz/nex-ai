namespace NexAI.Zendesk.Messages;

public record ZendeskUserGroupsImportedEvent(Guid UserId, Guid[] Groups) : IEvent;