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
        var answer = await chat.Ask("JUST SAY: TEST, nothing else.", "Hi", _cancellationTokenSource.Token);

        // assert
        answer.Should().Be("TEST");
    }

    [Fact]
    public async Task Ask_ThrowsException()
    {
        // arrange
        var chat = new FakeChat();

        // act
        var ask = async () => await chat.Ask<TestObject>("You are random data generator.", "Generate first and last name.", _cancellationTokenSource.Token);

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
        var response = chat.AskStream("JUST SAY: TEST, nothing else.", "Hi", _cancellationTokenSource.Token);
        await foreach (var message in response)
        {
            answer += message;
        }

        // assert
        answer.Should().Be("TEST");
    }
}