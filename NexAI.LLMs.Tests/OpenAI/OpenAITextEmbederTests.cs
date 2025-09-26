using NexAI.LLMs.Common;
using NexAI.LLMs.OpenAI;

namespace NexAI.LLMs.Tests.OpenAI;

public class OpenAITextEmbedderTests : LLMTestBase
{
    [Fact]
    public async Task GenerateEmbedding_ReturnEmbedding()
    {
        // arrange
        var embedder = new OpenAITextEmbedder(GetOptions(LLM.OpenAI));

        // act
        var embedding = await embedder.GenerateEmbedding("TEST");

        // assert
        embedding.Should().NotBeNull();
        embedding.ToArray().Should().NotBeEmpty();
    }
}