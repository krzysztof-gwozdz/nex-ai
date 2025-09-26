using NexAI.LLMs.Common;
using NexAI.LLMs.Ollama;

namespace NexAI.LLMs.Tests.Ollama;

public class OllamaChatTests : LLMTestBase
{
    [Fact]
    public async Task Ask_ReturnResponse()
    {
        // arrange
        var chat = new OllamaChat(GetOptions(LLM.Ollama));

        // act
        var answer = await chat.Ask("JUST SAY: TEST, nothing else.", "Hi");

        // assert
        answer.Should().Be("TEST");
    }  
    
    [Fact]
    public async Task AskStream_ReturnResponse()
    {
        // arrange
        var chat = new OllamaChat(GetOptions(LLM.Ollama));

        // act
        var answer = string.Empty;
        var response =  chat.AskStream("JUST SAY: TEST, nothing else.", "Hi");
        await foreach (var message in response)
        {
            answer += message;
        }

        // assert
        answer.Should().Be("TEST");
    }
}