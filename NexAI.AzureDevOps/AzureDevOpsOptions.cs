// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.ComponentModel.DataAnnotations;
using NexAI.Config;

namespace NexAI.AzureDevOps;

public class AzureDevOpsOptions : IOptions
{
    [Required(AllowEmptyStrings = false)]
    public Uri OrganizationUrl { get; init; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string ProjectName { get; init; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string PersonalAccessToken { get; init; } = null!;
}