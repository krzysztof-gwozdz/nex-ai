namespace NexAI.LLMs.Common;

public abstract class TextEmbedder
{
    public abstract ulong EmbeddingDimension { get; }

    public abstract Task<ReadOnlyMemory<float>> GenerateEmbedding(string text, CancellationToken cancellationToken);
}