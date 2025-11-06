using NexAI.Zendesk.Api.Dtos;

namespace NexAI.Zendesk;

public class ZendeskUserMapper
{
    public static ZendeskUser Map(UserDto user)
    {
        var zendeskUser = ZendeskUser.Create(
            NormalizeExternalId(user.Id),
            NormalizeName(user.Name),
            NormalizeEmail(user.Email)
        );
        return zendeskUser;
    }

    private static string NormalizeExternalId(long? id) =>
        id is null or < 0 ? throw new("Could not parse Id") : id.Value.ToString();

    private static string NormalizeName(string? name) =>
        string.IsNullOrWhiteSpace(name) ? "<MISSING NAME>" : name;

    private static string NormalizeEmail(string? email) =>
        string.IsNullOrWhiteSpace(email) ? throw new("Could not parse Email") : email;
}