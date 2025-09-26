using NexAI.LLMs.Common;
using NexAI.LLMs.Ollama;
using NexAI.LLMs.OpenAI;

namespace NexAI.LLMs.Tests.Common;

public class TextEmbedderTests : LLMTestBase
{
    [Fact]
    public void GetInstance_WithOpenAIMode_ReturnsOpenAITextEmbedder()
    {
        // arrange
        var options = GetOptions(LLM.OpenAI);

        // act
        var embedder = TextEmbedder.GetInstance(options);

        // assert
        embedder.Should().BeOfType<OpenAITextEmbedder>();
    }

    [Fact]
    public void GetInstance_WithOllamaMode_ReturnsOllamaTextEmbedder()
    {
        // arrange
        var options = GetOptions(LLM.Ollama);

        // act
        var embedder = TextEmbedder.GetInstance(options);

        // assert
        embedder.Should().BeOfType<OllamaTextEmbedder>();
    }

    [Fact]
    public void GetInstance_WithUnknownMode_Throws()
    {
        // arrange
        var options = GetOptions("Unknown");

        // act
        var getInstance = () => TextEmbedder.GetInstance(options);

        // assert
        getInstance.Should().Throw<Exception>().WithMessage("Unknown LLM or unsupported mode: Unknown");
    }
}