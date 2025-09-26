using NexAI.Config;
using NexAI.LLMs.Common;
using OllamaSharp;

namespace NexAI.LLMs.Ollama;

public class OllamaTextEmbedder(Options options) : TextEmbedder
{
    private readonly OllamaApiClient _apiClient = new(
        options.Get<OllamaOptions>().BaseAddress,
        options.Get<OllamaOptions>().EmbeddingModel
    );
    
    public override ulong EmbeddingDimension => options.Get<OllamaOptions>().EmbeddingDimension;

    public override async Task<ReadOnlyMemory<float>> GenerateEmbedding(string text)
    {
        var embedding = await _apiClient.EmbedAsync(text);
        return embedding.Embeddings.First().ToArray();
    }
}