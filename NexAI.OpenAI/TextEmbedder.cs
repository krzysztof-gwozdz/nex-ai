using NexAI.Config;
using OpenAI.Embeddings;

namespace NexAI.OpenAI;

public class TextEmbedder
{
    private readonly EmbeddingClient _embeddingClient;

    public TextEmbedder(Options options)
    {
        var apiKey = options.Get<OpenAIOptions>().ApiKey;
        _embeddingClient = new("text-embedding-3-small", apiKey); // 1536 dimensions
    }

    public async Task<ReadOnlyMemory<float>> GenerateEmbedding(string text)
    {
        var embedding = await _embeddingClient.GenerateEmbeddingAsync(text);
        return embedding.Value.ToFloats();
    }
}