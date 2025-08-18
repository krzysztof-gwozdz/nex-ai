using System.ComponentModel.DataAnnotations;
using NexAI.Config;

namespace NexAI.LLMs;

public class LLMsOptions : IOptions
{
    [Required(AllowEmptyStrings = false)]
    public string Mode { get; init; } = null!;
}

public abstract class TextEmbedder
{
    public abstract ulong EmbeddingDimension { get; }

    public abstract Task<ReadOnlyMemory<float>> GenerateEmbedding(string text);
    
    public static TextEmbedder GetInstance(Options options) => options.Get<LLMsOptions>().Mode switch
    {
        "OpenAI" => new OpenAITextEmbedder(options),
        "Ollama" => new OllamaTextEmbedder(options),
        _ => throw new Exception($"Unknown LLM mode: {options.Get<LLMsOptions>().Mode}")
    };
}