using FluentAssertions;
using Xunit;

namespace NexAI.Zendesk.Tests;

public class ZendeskTicketTests
{
    [Fact]
    public void Create_GeneratesNewId()
    {
        // arrange

        // act
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

        // assert
        zendeskTicket.Id.Value.Should().NotBeEmpty();
        zendeskTicket.Id.Value.Version.Should().Be(7);
    } 
    
    [Theory]
    [InlineData(null, null)]
    [InlineData("test category", "test category")]
    [InlineData("test__category", "test")]
    [InlineData("test__category__subcategory", "test")]
    public void Create_SetValidMainCategory(string? category, string? expectedMainCategory)
    {
        // arrange

        // act
        var zendeskTicket = ZendeskTicket.Create(
            "test externalId",
            "test title",
            "test description",
            "http://test.url",
            category,
            "open",
            "test country",
            "test merchantId",
            ["tag1", "tag2"],
            DateTime.UtcNow,
            null,
            []);

        // assert
        zendeskTicket.MainCategory.Should().Be(expectedMainCategory);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("a", "team_l3_a")]
    [InlineData("a", "team_l2_hackers", "team_l3_a")]
    [InlineData("a", "team_l3_a", "team_l3_b")]
    public void Create_SetValidLevel3Team(string? expectedLevel3Team, params string[] tags)
    {
        // arrange

        // act
        var zendeskTicket = ZendeskTicket.Create(
            "test externalId",
            "test title",
            "test description",
            "http://test.url",
            "test category",
            "open",
            "test country",
            "test merchantId",
            tags,
            DateTime.UtcNow,
            null,
            []);

        // assert
        zendeskTicket.Level3Team.Should().Be(expectedLevel3Team);
    }
}