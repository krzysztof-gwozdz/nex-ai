using NexAI.Config;
using NexAI.LLMs.Ollama;
using NexAI.LLMs.OpenAI;

namespace NexAI.LLMs.Common;

public abstract class TextEmbedder
{
    public abstract ulong EmbeddingDimension { get; }

    public abstract Task<ReadOnlyMemory<float>> GenerateEmbedding(string text);
    
    public static TextEmbedder GetInstance(Options options) => options.Get<LLMsOptions>().Mode switch
    {
        LLM.OpenAI => new OpenAITextEmbedder(options),
        LLM.Ollama => new OllamaTextEmbedder(options),
        _ => throw new($"Unknown LLM mode: {options.Get<LLMsOptions>().Mode}")
    };
}