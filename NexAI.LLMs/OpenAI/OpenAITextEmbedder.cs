using NexAI.Config;
using NexAI.LLMs.Common;
using OpenAI.Embeddings;

namespace NexAI.LLMs.OpenAI;

public class OpenAITextEmbedder(Options options) : TextEmbedder
{
    private readonly EmbeddingClient _embeddingClient = new(
        options.Get<OpenAIOptions>().EmbeddingModel,
        options.Get<OpenAIOptions>().ApiKey
    );
    
    public override ulong EmbeddingDimension => options.Get<OpenAIOptions>().EmbeddingDimension;

    public override async Task<ReadOnlyMemory<float>> GenerateEmbedding(string text)
    {
        var embedding = await _embeddingClient.GenerateEmbeddingAsync(text);
        return embedding.Value.ToFloats();
    }
}