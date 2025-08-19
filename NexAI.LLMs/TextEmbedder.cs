using NexAI.Config;

namespace NexAI.LLMs;

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