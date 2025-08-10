using System.ComponentModel.DataAnnotations;
using NexAI.Config;

namespace NexAI.OpenAI;

public record OpenAIOptions : IOptions
{
    [Required(AllowEmptyStrings = false)]
    public string ApiKey { get; init; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string Model { get; init; } = null!;
}