using NexAI.Config;
using OllamaSharp;

namespace NexAI.Ollama;

public class TextEmbedder(Options options)
{
    private readonly OllamaApiClient _apiClient = new(
        options.Get<OllamaOptions>().BaseAddress,
        options.Get<OllamaOptions>().EmbeddingModel
    );
    
    public ulong EmbeddingDimension => options.Get<OllamaOptions>().EmbeddingDimension;

    public async Task<ReadOnlyMemory<float>> GenerateEmbedding(string text)
    {
        var embedding = await _apiClient.EmbedAsync(text);
        return embedding.Embeddings.First().ToArray();
    }
}