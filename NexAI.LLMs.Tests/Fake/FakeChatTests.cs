using NexAI.LLMs.Common;
using NexAI.LLMs.Fake;
using NexAI.LLMs.Ollama;

namespace NexAI.LLMs.Tests.Fake;

public class FakeChatTests : LLMTestBase
{
    private readonly CancellationTokenSource _cancellationTokenSource = new(TimeSpan.FromSeconds(60));

    [Fact]
    public async Task Ask_ReturnResponse()
    {
        // arrange
        var chat = new FakeChat();

        // act
        var answer = await chat.Ask(ConversationId.New(), "JUST SAY: TEST, nothing else.", "Hi", _cancellationTokenSource.Token);

        // assert
        answer.Should().Be("TEST");
    }

    [Fact]
    public async Task Ask_ThrowsException()
    {
        // arrange
        var chat = new FakeChat();

        // act
        var ask = async () => await chat.Ask<TestObject>(ConversationId.New(), "You are random data generator.", "Generate first and last name.", _cancellationTokenSource.Token);

        // assert
        await ask.Should().ThrowExactlyAsync<NotSupportedException>();
    }

    [Fact]
    public async Task AskStream_ReturnResponse()
    {
        // arrange
        var chat = new FakeChat();

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