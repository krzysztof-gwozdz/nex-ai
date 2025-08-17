using System.Text.Json.Serialization;

namespace NexAI.Zendesk.Api;

public record ListUsersDto(
    [property: JsonPropertyName("users")]
    ListUsersDto.UserDto[]? Users,
    string? NextPage,
    string? PreviousPage,
    int? Count) : PagedDto(NextPage, PreviousPage, Count)
{
    public record UserDto(
        [property: JsonPropertyName("id")]
        long? Id,
        [property: JsonPropertyName("url")]
        string? Url,
        [property: JsonPropertyName("name")]
        string? Name,
        [property: JsonPropertyName("email")]
        string? Email,
        [property: JsonPropertyName("created_at")]
        string? CreatedAt,
        [property: JsonPropertyName("updated_at")]
        string? UpdatedAt,
        [property: JsonPropertyName("time_zone")]
        string? TimeZone,
        [property: JsonPropertyName("iana_time_zone")]
        string? IanaTimeZone,
        [property: JsonPropertyName("phone")]
        string? Phone,
        [property: JsonPropertyName("shared_phone_number")]
        bool? SharedPhoneNumber,
        [property: JsonPropertyName("photo")]
        object? Photo,
        [property: JsonPropertyName("remote_photo_url")]
        string? RemotePhotoUrl,
        [property: JsonPropertyName("chat_only")]
        bool? ChatOnly,
        [property: JsonPropertyName("locale_id")]
        long? LocaleId,
        [property: JsonPropertyName("locale")]
        string? Locale,
        [property: JsonPropertyName("organization_id")]
        long? OrganizationId,
        [property: JsonPropertyName("role")]
        string? Role,
        [property: JsonPropertyName("verified")]
        bool? Verified,
        [property: JsonPropertyName("external_id")]
        string? ExternalId,
        [property: JsonPropertyName("tags")]
        string[]? Tags,
        [property: JsonPropertyName("alias")]
        string? Alias,
        [property: JsonPropertyName("active")]
        bool? Active,
        [property: JsonPropertyName("shared")]
        bool? Shared,
        [property: JsonPropertyName("shared_agent")]
        bool? SharedAgent,
        [property: JsonPropertyName("last_login_at")]
        string? LastLoginAt,
        [property: JsonPropertyName("two_factor_auth_enabled")]
        bool? TwoFactorAuthEnabled,
        [property: JsonPropertyName("signature")]
        string? Signature,
        [property: JsonPropertyName("details")]
        string? Details,
        [property: JsonPropertyName("notes")]
        string? Notes,
        [property: JsonPropertyName("role_type")]
        long? RoleType,
        [property: JsonPropertyName("custom_role_id")]
        long? CustomRoleId,
        [property: JsonPropertyName("moderator")]
        bool? Moderator,
        [property: JsonPropertyName("ticket_restriction")]
        string? TicketRestriction,
        [property: JsonPropertyName("only_private_comments")]
        bool? OnlyPrivateComments,
        [property: JsonPropertyName("restricted_agent")]
        bool? RestrictedAgent,
        [property: JsonPropertyName("suspended")]
        bool? Suspended,
        [property: JsonPropertyName("default_group_id")]
        long? DefaultGroupId,
        [property: JsonPropertyName("report_csv")]
        bool? ReportCsv,
        [property: JsonPropertyName("user_fields")]
        UserDto.UserFieldsDto? UserFields)
    {
        public record UserFieldsDto(
            [property: JsonPropertyName("company_name")]
            string? CompanyName,
            [property: JsonPropertyName("merchantid")]
            long? MerchantId,
            [property: JsonPropertyName("orgnr")]
            string? Organization,
            [property: JsonPropertyName("phone_number")]
            string? PhoneNumber,
            [property: JsonPropertyName("url")]
            string? Url,
            [property: JsonPropertyName("user_brand")]
            string? UserBrand);
    }
}