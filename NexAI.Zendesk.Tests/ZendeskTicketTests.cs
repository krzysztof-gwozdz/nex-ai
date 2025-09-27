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
    public void Create_WithDifferentCategories_SetValidMainCategory(string? category, string? expectedMainCategory)
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
}