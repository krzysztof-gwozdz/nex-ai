// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.ComponentModel.DataAnnotations;
using NexAI.Config;

namespace NexAI.LLMs.Ollama;

public record OllamaOptions : IOptions
{
    [Required(AllowEmptyStrings = false)]
    public Uri BaseAddress { get; init; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string ChatModel { get; init; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string EmbeddingModel { get; init; } = null!;
    
    [Required]
    public ulong EmbeddingDimension { get; init; }
}