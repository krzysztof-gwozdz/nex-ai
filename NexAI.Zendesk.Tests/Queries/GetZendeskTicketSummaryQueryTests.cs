using System.Text.Json;
using FluentAssertions;
using NexAI.LLMs;
using NexAI.Zendesk.Queries;
using NexAI.Zendesk.Tests.Builders;
using Xunit;

namespace NexAI.Zendesk.Tests.Queries;

public class GetZendeskTicketSummaryQueryTests
{
    [Fact]
    public async Task Handle_WithFakeChat_ReturnsSummaryFromChat()
    {
        // arrange
        const string expectedSummary = "Customer requested a refund for order #12345.";
        var zendeskTicket = ZendeskTicketBuilder.Create().WithTitle("Refund request").WithDescription("I need a refund.").Build();
        var chat = new FakeChat(expectedSummary);
        var query = new GetZendeskTicketSummaryQuery(chat, new PromptReader());

        // act
        var result = await query.Handle(zendeskTicket, CancellationToken.None);
        
        // assert
        result.Should().Be(expectedSummary);
        chat.Messages[0].Role.Should().Be("system");
        chat.Messages[0].Content.Should().Be(
@"Summarize the following Zendesk ticket into a concise summary that captures the main issue, key details, and any relevant context. 
The summary should be clear and informative, suitable for a quick understanding of the ticket's content.
It should be no longer than 4 sentences.
Do not include any personal data or sensitive information.
It should be in English.
Do not ask questions. 
Do not use emojis.
Do not include headings.
Only include information that is present in the ticket.
If the ticket is empty or does not contain enough information to generate a summary, respond with: ""No sufficient information to generate a summary.""
The ticket is in JSON format.
Response format:
{
    ""summary"": ""string"",
    ""languages"": [""string"" /* detected languages in the ticket, e.g., English, Spanish */]
}");
        chat.Messages[1].Role.Should().Be("user");
        chat.Messages[1].Content.Should().Be(JsonSerializer.Serialize(zendeskTicket, new JsonSerializerOptions { WriteIndented = true }));
    }
}