using NexAI.LLMs.Common;
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
        var answer = await chat.Ask(ConversationId.New(), "JUST SAY: TEST, nothing else.", "Hi", _cancellationTokenSource.Token);

        // assert
        answer.Should().Be("TEST");
    }

    [Fact]
    public async Task Ask_ReturnStructuredResponse()
    {
        // arrange
        var chat = new OllamaChat(GetOptions());

        // act
        var testObject = await chat.Ask<TestObject>(ConversationId.New(), "You are random data generator.", "Generate first and last name.", _cancellationTokenSource.Token);

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
        var response = chat.AskStream(ConversationId.New(), "JUST SAY: TEST, nothing else.", "Hi", _cancellationTokenSource.Token);
        await foreach (var message in response)
        {
            answer += message;
        }

        // assert
        answer.Should().Be("TEST");
    }
    
    [Fact]
    public async Task GetNextResponse_ReturnResponse()
    {
        // arrange
        var chat = new OllamaChat(GetOptions());
        var messages = new[] { new ChatMessage("system", "JUST SAY: TEST, nothing else."), new ChatMessage("user", "Hi") };

        // act
        var answer = await chat.GetNextResponse(ConversationId.New(), messages, _cancellationTokenSource.Token);

        // assert
        answer.Should().Be("TEST");
    }

    [Fact]
    public async Task StreamNextResponse_ReturnResponse()
    {
        // arrange
        var chat = new OllamaChat(GetOptions());
        var messages = new[] { new ChatMessage("system", "JUST SAY: TEST, nothing else."), new ChatMessage("user", "Hi") };

        // act
        var answer = string.Empty;
        var response = chat.StreamNextResponse(ConversationId.New(), messages, _cancellationTokenSource.Token);
        await foreach (var message in response)
        {
            answer += message;
        }

        // assert
        answer.Should().Be("TEST");
    }
}