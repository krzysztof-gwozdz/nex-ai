using NexAI.Zendesk.Messages;

namespace NexAI.Zendesk;

public record ZendeskUserGroups(ZendeskUserId UserId, ZendeskGroupId[] Groups)
{
    public static ZendeskUserGroups FromZendeskUserGroupsImportedEvent(ZendeskUserGroupsImportedEvent zendeskUserGroupsImportedEvent) =>
        new(
            new(zendeskUserGroupsImportedEvent.UserId),
            zendeskUserGroupsImportedEvent.Groups.Select(groupId => new ZendeskGroupId(groupId)).ToArray()
        );
    
    public ZendeskUserGroupsImportedEvent ToZendeskUserGroupsImportedEvent() =>
        new(UserId.Value, Groups.Select(groupId => groupId.Value).ToArray());
}