using System.Net;
using NexAI.Api.Controllers;

namespace NexAI.Api.Tests.Controllers;

public sealed class ChatTests(NexAIApiApplicationFactory factory) : TestsBase(factory)
{
    private const string Url = "/chat";
    
    [Fact]
    public async Task Post_WithValidBodyAndStreamOff_ReturnsOkAndExpectedContent()
    {
        // arrange
        var request = new ChatRequest([new ChatRequest.Message("user", "hello")], Stream: false);

        // act
        var response = await Post(Url, request);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await GetResponse<string>(response);
        body.Should().Be("I'm sorry, I can't help with that.");
    }

    [Fact]
    public async Task Post_WithValidBodyAndStreamOn_ReturnsOkAndExpectedContent()
    {
        // arrange
        var request = new ChatRequest([new ChatRequest.Message("user", "hello")], Stream: true);

        // act
        var response = await Post(Url, request);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var chunks = await GetResponse<string[]>(response);
        chunks.Should().NotBeNull();
        var combined = string.Concat(chunks);
        combined.Should().Be("I'm sorry, I can't help with that.");
    }
}
