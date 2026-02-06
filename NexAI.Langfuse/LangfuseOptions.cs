using System.ComponentModel.DataAnnotations;
using NexAI.Config;

namespace NexAI.Langfuse;

public record LangfuseOptions : IOptions
{
    [Required(AllowEmptyStrings = false)]
    public string PublicKey { get; init; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string SecretKey { get; init; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string Url { get; init; } = null!;
}