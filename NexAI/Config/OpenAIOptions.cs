using System.ComponentModel.DataAnnotations;

namespace NexAI.Config;

public record OpenAIOptions : IOptions
{
    [Required(AllowEmptyStrings = false)]
    public string ApiKey { get; init; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string Model { get; init; } = null!;
}