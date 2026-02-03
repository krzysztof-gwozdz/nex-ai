using System.Net;
using NexAI.Api.Controllers;

namespace NexAI.Api.Tests.Controllers;

public sealed class AgentTests(NexAIApiApplicationFactory factory) : TestsBase(factory)
{
    [Fact]
    public async Task Post_WithValidBodyAndStreamOff_ReturnsOkAndExpectedContent()
    {
        // arrange
        var request = new AgentRequest([new AgentRequest.Message("user", "hello")], Stream: false);

        // act
        var response = await Post("/agent", request);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await GetResponse<string>(response);
        body.Should().Be("Fake agent response");
    }

    [Fact]
    public async Task Post_WithValidBodyAndStreamOn_ReturnsOkAndExpectedContent()
    {
        // arrange
        var request = new AgentRequest([new AgentRequest.Message("user", "hello")], Stream: true);

        // act
        var response = await Post("/agent", request);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var chunks = await GetResponse<string[]>(response);
        chunks.Should().NotBeNull();
        var combined = string.Concat(chunks);
        combined.Should().Be("Fake agent response");
    }
}