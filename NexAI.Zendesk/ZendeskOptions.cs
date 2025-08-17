using System.ComponentModel.DataAnnotations;
using System.Text;
using NexAI.Config;

namespace NexAI.Zendesk;

public record ZendeskOptions : IOptions
{
    [Required(AllowEmptyStrings = false)]
    public string ApiBaseUrl { get; init; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string ApiAuthEmailAddress { get; init; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string ApiToken { get; init; } = null!;

    public string AuthorizationToken => 
        Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ApiAuthEmailAddress}/token:{ApiToken}"));
}