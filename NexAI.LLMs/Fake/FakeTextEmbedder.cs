using NexAI.LLMs.Common;

namespace NexAI.LLMs.Fake;

public class FakeTextEmbedder: TextEmbedder
{
    public override ulong EmbeddingDimension => 16;

    public override Task<ReadOnlyMemory<float>> GenerateEmbedding(string text, CancellationToken cancellationToken) => 
        Task.FromResult<ReadOnlyMemory<float>>(new([1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 10.0f, 11.0f, 12.0f, 13.0f, 14.0f, 15.0f, 16.0f]));
}