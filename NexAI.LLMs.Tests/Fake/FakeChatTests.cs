using NexAI.LLMs.Common;
using NexAI.LLMs.Fake;

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
    public async Task Ask_ReturnStructuredResponse()
    {
        // arrange
        var chat = new FakeChat();

        // act
        var answer = await chat.Ask<string>(ConversationId.New(), "JUST SAY: TEST, nothing else.", "Hi", _cancellationTokenSource.Token);

        // assert
        answer.Should().Be("TEST");
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
        var chat = new FakeChat();
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
        var chat = new FakeChat();
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