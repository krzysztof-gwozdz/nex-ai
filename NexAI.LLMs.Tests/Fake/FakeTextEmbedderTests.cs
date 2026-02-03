using NexAI.LLMs.Fake;

namespace NexAI.LLMs.Tests.Fake;

public class FakeTextEmbedderTests : LLMTestBase
{
    private readonly CancellationTokenSource _cancellationTokenSource = new(TimeSpan.FromSeconds(60));
    
    [Fact]
    public async Task GenerateEmbedding_ReturnEmbedding()
    {
        // arrange
        var embedder = new FakeTextEmbedder();

        // act
        var embedding = await embedder.GenerateEmbedding("TEST", _cancellationTokenSource.Token);

        // assert
        embedding.Should().NotBeNull();
        embedding.ToArray().Should().NotBeEmpty();
    }
}