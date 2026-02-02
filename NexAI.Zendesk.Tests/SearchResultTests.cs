using FluentAssertions;
using Xunit;

namespace NexAI.Zendesk.Tests;

public class SearchResultTests
{
    [Fact]
    public void FullTextSearchResult_WithValidArguments_SetsExpectedProperties()
    {
        // arrange
        var zendeskTicket = ZendeskTicket.Create(
            "test externalId",
            "test title",
            "test description",
            "http://test.url",
            "test category",
            "open",
            "test country",
            "test merchantId",
            ["tag1", "tag2"],
            DateTime.UtcNow,
            null,
            []);
        const double score = 0.95;

        // act
        var result = SearchResult.FullTextSearchResult(zendeskTicket, score);

        // assert
        result.ZendeskTicket.Should().Be(zendeskTicket);
        result.Score.Should().Be(score);
        result.Method.Should().Be("full-text");
        result.Info.Should().Be("");
    }

    [Fact]
    public void EmbeddingBasedSearchResult_WithValidArguments_SetsExpectedProperties()
    {
        // arrange
        var zendeskTicket = ZendeskTicket.Create(
            "test externalId",
            "test title",
            "test description",
            "http://test.url",
            "test category",
            "open",
            "test country",
            "test merchantId",
            ["tag1", "tag2"],
            DateTime.UtcNow,
            null,
            []);
        const double score = 0.87;
        const string info = "similarity info";

        // act
        var result = SearchResult.EmbeddingBasedSearchResult(zendeskTicket, score, info);

        // assert
        result.ZendeskTicket.Should().Be(zendeskTicket);
        result.Score.Should().Be(score);
        result.Method.Should().Be("embedding-based");
        result.Info.Should().Be(info);
    }

    [Fact]
    public void Equal_SameValue_ProducesEqualRecords()
    {
        // arrange
        var zendeskTicket = ZendeskTicket.Create(
            "test externalId",
            "test title",
            "test description",
            "http://test.url",
            "test category",
            "open",
            "test country",
            "test merchantId",
            ["tag1", "tag2"],
            DateTime.UtcNow,
            null,
            []);
        const double score = 0.9;
        const string method = "full-text";
        const string info = "";
        var result1 = new SearchResult(zendeskTicket, score, method, info);
        var result2 = new SearchResult(zendeskTicket, score, method, info);

        // act & assert
        result1.Should().Be(result2);
        (result1 == result2).Should().BeTrue();
    }

    [Fact]
    public void Equal_DifferentValues_ProducesUnequalRecords()
    {
        // arrange
        var zendeskTicket = ZendeskTicket.Create(
            "test externalId",
            "test title",
            "test description",
            "http://test.url",
            "test category",
            "open",
            "test country",
            "test merchantId",
            ["tag1", "tag2"],
            DateTime.UtcNow,
            null,
            []);
        var result1 = new SearchResult(zendeskTicket, 0.9, "full-text", "");
        var result2 = new SearchResult(zendeskTicket, 0.8, "embedding-based", "info");

        // act & assert
        result1.Should().NotBe(result2);
        (result1 == result2).Should().BeFalse();
    }
}
