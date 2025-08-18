using System.ComponentModel.DataAnnotations;
using NexAI.Config;

namespace NexAI.LLMs;

public record OpenAIOptions : IOptions
{
    [Required(AllowEmptyStrings = false)]
    public string ApiKey { get; init; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string ChatModel { get; init; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string EmbeddingModel { get; init; } = null!;
    
    public ulong EmbeddingDimension { get; init; }
}