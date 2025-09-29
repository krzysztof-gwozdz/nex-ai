using NexAI.LLMs.OpenAI;

namespace NexAI.LLMs.Tests.OpenAI;

public class OpenAITextEmbedderTests : LLMTestBase
{
    private readonly CancellationTokenSource _cancellationTokenSource = new(TimeSpan.FromSeconds(60));
    
    [Fact]
    public async Task GenerateEmbedding_ReturnEmbedding()
    {
        // arrange
        var embedder = new OpenAITextEmbedder(GetOptions());

        // act
        var embedding = await embedder.GenerateEmbedding("TEST", _cancellationTokenSource.Token);

        // assert
        embedding.Should().NotBeNull();
        embedding.ToArray().Should().NotBeEmpty();
    }
}