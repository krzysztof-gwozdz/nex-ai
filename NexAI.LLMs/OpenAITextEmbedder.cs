using NexAI.Config;
using OpenAI.Embeddings;

namespace NexAI.LLMs;

public class OpenAITextEmbedder(Options options)
{
    private readonly EmbeddingClient _embeddingClient = new(
        options.Get<OpenAIOptions>().EmbeddingModel,
        options.Get<OpenAIOptions>().ApiKey
    );
    
    public ulong EmbeddingDimension => options.Get<OpenAIOptions>().EmbeddingDimension;

    public async Task<ReadOnlyMemory<float>> GenerateEmbedding(string text)
    {
        var embedding = await _embeddingClient.GenerateEmbeddingAsync(text);
        return embedding.Value.ToFloats();
    }
}