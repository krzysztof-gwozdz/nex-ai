using NexAI.LLMs.Ollama;

namespace NexAI.LLMs.Tests.Ollama;

public class OllamaTextEmbedderTests : LLMTestBase
{
    [Fact]
    public async Task GenerateEmbedding_ReturnEmbedding()
    {
        // arrange
        var embedder = new OllamaTextEmbedder(GetOptions());

        // act
        var embedding = await embedder.GenerateEmbedding("TEST");

        // assert
        embedding.Should().NotBeNull();
        embedding.ToArray().Should().NotBeEmpty();
    }
}