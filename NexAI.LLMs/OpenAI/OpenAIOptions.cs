// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.ComponentModel.DataAnnotations;
using NexAI.Config;

namespace NexAI.LLMs.OpenAI;

public record OpenAIOptions : IOptions
{
    [Required(AllowEmptyStrings = false)]
    public string ApiKey { get; init; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string ChatModel { get; init; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string EmbeddingModel { get; init; } = null!;
    
    [Required]
    public ulong EmbeddingDimension { get; init; }
}