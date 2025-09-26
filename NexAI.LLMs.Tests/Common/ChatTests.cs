using NexAI.LLMs.Common;
using NexAI.LLMs.Ollama;
using NexAI.LLMs.OpenAI;

namespace NexAI.LLMs.Tests.Common;

public class ChatTests : LLMTestBase
{
    [Fact]
    public void GetInstance_WithOpenAIMode_ReturnsOpenAIChat()
    {
        // arrange
        var options = GetOptions(LLM.OpenAI);

        // act
        var chat = Chat.GetInstance(options);

        // assert
        chat.Should().BeOfType<OpenAIChat>();
    }

    [Fact]
    public void GetInstance_WithOllamaMode_ReturnsOllamaChat()
    {
        // arrange
        var options = GetOptions(LLM.Ollama);

        // act
        var chat = Chat.GetInstance(options);

        // assert
        chat.Should().BeOfType<OllamaChat>();
    }

    [Fact]
    public void GetInstance_WithUnknownMode_Throws()
    {
        // arrange
        var options = GetOptions("Unknown");

        // act
        var getInstance = () => Chat.GetInstance(options);

        // assert
        getInstance.Should().Throw<Exception>().WithMessage("Unknown LLM or unsupported mode: Unknown");
    }
}