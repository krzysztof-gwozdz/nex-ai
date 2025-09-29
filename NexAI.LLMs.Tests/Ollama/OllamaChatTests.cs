using NexAI.LLMs.Ollama;

namespace NexAI.LLMs.Tests.Ollama;

public class OllamaChatTests : LLMTestBase
{
    private readonly CancellationTokenSource _cancellationTokenSource = new(TimeSpan.FromSeconds(60));

    [Fact]
    public async Task Ask_ReturnResponse()
    {
        // arrange
        var chat = new OllamaChat(GetOptions());

        // act
        var answer = await chat.Ask("JUST SAY: TEST, nothing else.", "Hi", _cancellationTokenSource.Token);

        // assert
        answer.Should().Be("TEST");
    }

    [Fact]
    public async Task Ask_ReturnStructuredResponse()
    {
        // arrange
        var chat = new OllamaChat(GetOptions());

        // act
        var testObject = await chat.Ask<TestObject>("You are random data generator.", "Generate first and last name.", _cancellationTokenSource.Token);

        // assert
        testObject.FirstName.Should().NotBeEmpty();
        testObject.LastName.Should().NotBeEmpty();
    }

    [Fact]
    public async Task AskStream_ReturnResponse()
    {
        // arrange
        var chat = new OllamaChat(GetOptions());

        // act
        var answer = string.Empty;
        var response = chat.AskStream("JUST SAY: TEST, nothing else.", "Hi", _cancellationTokenSource.Token);
        await foreach (var message in response)
        {
            answer += message;
        }

        // assert
        answer.Should().Be("TEST");
    }
}