using NexAI.Zendesk.Api.Dtos;

namespace NexAI.Zendesk;

public class ZendeskGroupMapper
{
    public static ZendeskGroup Map(GroupDto group)
    {
        var zendeskGroup = ZendeskGroup.Create(
            NormalizeExternalId(group.Id),
            NormalizeName(group.Name)
        );
        return zendeskGroup;
    }

    private static string NormalizeExternalId(long? id) =>
        id is null or < 0 ? throw new("Could not parse Id") : id.Value.ToString();

    private static string NormalizeName(string? name) =>
        string.IsNullOrWhiteSpace(name) ? "<MISSING NAME>" : name;
}