using FluentAssertions;
using NexAI.Zendesk.Messages;
using Xunit;

namespace NexAI.Zendesk.Tests;

public class ZendeskTicketTests
{
    [Fact]
    public void Constructor_AssignsAllProperties()
    {
        // arrange
        var id = ZendeskTicketId.New();
        const string externalId = "ext-123";
        const string title = "Ticket Title";
        const string description = "Ticket description";
        const string url = "https://example.com/ticket/1";
        const string? mainCategory = "Main";
        const string? category = "Main__Sub";
        const string status = "open";
        const string? country = "NO";
        const string? merchantId = "merchant-1";
        const string? level3Team = "team-a";
        var tags = new[] { "tag1", "tag2" };
        var createdAt = new DateTime(2025, 2, 1, 10, 0, 0, DateTimeKind.Utc);
        var updatedAt = new DateTime(2025, 2, 2, 12, 0, 0, DateTimeKind.Utc);
        var messageId = ZendeskTicketMessageId.New();
        var messages = new[]
        {
            new ZendeskTicket.ZendeskTicketMessage(messageId, "msg-ext-1", "Content", "Author", createdAt)
        };

        // act
        var ticket = new ZendeskTicket(
            id,
            externalId,
            title,
            description,
            url,
            mainCategory,
            category,
            status,
            country,
            merchantId,
            level3Team,
            tags,
            createdAt,
            updatedAt,
            messages);

        // assert
        ticket.Id.Should().Be(id);
        ticket.ExternalId.Should().Be(externalId);
        ticket.Title.Should().Be(title);
        ticket.Description.Should().Be(description);
        ticket.Url.Should().Be(url);
        ticket.MainCategory.Should().Be(mainCategory);
        ticket.Category.Should().Be(category);
        ticket.Status.Should().Be(status);
        ticket.Country.Should().Be(country);
        ticket.MerchantId.Should().Be(merchantId);
        ticket.Level3Team.Should().Be(level3Team);
        ticket.Tags.Should().BeSameAs(tags);
        ticket.CreatedAt.Should().Be(createdAt);
        ticket.UpdatedAt.Should().Be(updatedAt);
        ticket.Messages.Should().BeSameAs(messages);
        ticket.Messages[0].Id.Should().Be(messageId);
        ticket.Messages[0].ExternalId.Should().Be("msg-ext-1");
        ticket.Messages[0].Content.Should().Be("Content");
        ticket.Messages[0].Author.Should().Be("Author");
        ticket.Messages[0].CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public void Constructor_WithSameValues_ProducesEqualRecords()
    {
        // arrange
        var id = ZendeskTicketId.New();
        var tags = new[] { "tag1" };
        var createdAt = DateTime.UtcNow;
        var messages = Array.Empty<ZendeskTicket.ZendeskTicketMessage>();
        var ticket1 = new ZendeskTicket(
            id,
            "ext",
            "Title",
            "Desc",
            "https://url",
            null,
            null,
            "open",
            null,
            null,
            null,
            tags,
            createdAt,
            null,
            messages);
        var ticket2 = new ZendeskTicket(
            id,
            "ext",
            "Title",
            "Desc",
            "https://url",
            null,
            null,
            "open",
            null,
            null,
            null,
            tags,
            createdAt,
            null,
            messages);

        // act & assert
        ticket1.Should().Be(ticket2);
        (ticket1 == ticket2).Should().BeTrue();
    }

    [Fact]
    public void Create_ReturnsTicketWithGivenValuesAndNonEmptyId()
    {
        // arrange
        const string externalId = "create-ext";
        const string title = "Create Title";
        const string description = "Create description";
        const string url = "https://create.url";
        const string category = "main__sub";
        const string status = "pending";
        const string country = "SE";
        const string merchantId = "m-1";
        var tags = new[] { "team_l3_foo", "other" };
        var createdAt = new DateTime(2025, 1, 15, 14, 0, 0, DateTimeKind.Utc);
        var updatedAt = new DateTime(2025, 1, 16, 10, 0, 0, DateTimeKind.Utc);
        var messages = Array.Empty<ZendeskTicket.ZendeskTicketMessage>();

        // act
        var ticket = ZendeskTicket.Create(
            externalId,
            title,
            description,
            url,
            category,
            status,
            country,
            merchantId,
            tags,
            createdAt,
            updatedAt,
            messages);

        // assert
        ticket.Id.Value.Should().NotBe(Guid.Empty);
        ticket.ExternalId.Should().Be(externalId);
        ticket.Title.Should().Be(title);
        ticket.Description.Should().Be(description);
        ticket.Url.Should().Be(url);
        ticket.MainCategory.Should().Be("main");
        ticket.Category.Should().Be(category);
        ticket.Status.Should().Be(status);
        ticket.Country.Should().Be(country);
        ticket.MerchantId.Should().Be(merchantId);
        ticket.Level3Team.Should().Be("foo");
        ticket.Tags.Should().BeSameAs(tags);
        ticket.CreatedAt.Should().Be(createdAt);
        ticket.UpdatedAt.Should().Be(updatedAt);
        ticket.Messages.Should().BeSameAs(messages);
    }

    [Fact]
    public void Create_EachCall_GeneratesNewId()
    {
        // arrange & act
        var ticket1 = ZendeskTicket.Create(
            "ext-1",
            "Title 1",
            "Desc 1",
            "https://url/1",
            null,
            "open",
            null,
            null,
            [],
            DateTime.UtcNow,
            null,
            []);
        var ticket2 = ZendeskTicket.Create(
            "ext-2",
            "Title 2",
            "Desc 2",
            "https://url/2",
            null,
            "open",
            null,
            null,
            [],
            DateTime.UtcNow,
            null,
            []);

        // assert
        ticket1.Id.Value.Should().NotBe(ticket2.Id.Value);
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

    [Fact]
    public void FromZendeskTicketImportedEvent_ReturnsTicketWithEventData()
    {
        // arrange
        var ticketId = Guid.NewGuid();
        const string externalId = "event-ext";
        const string title = "Event Title";
        const string description = "Event description";
        const string url = "https://event.url";
        const string? mainCategory = "Main";
        const string? category = "Sub";
        const string status = "solved";
        const string? country = "DK";
        const string? merchantId = "m-event";
        const string? level3Team = "l3";
        var tags = new[] { "t1", "t2" };
        var createdAt = new DateTime(2025, 2, 1, 8, 0, 0, DateTimeKind.Utc);
        var updatedAt = new DateTime(2025, 2, 1, 9, 0, 0, DateTimeKind.Utc);
        var messageId = Guid.NewGuid();
        var messageCreatedAt = new DateTime(2025, 2, 1, 8, 30, 0, DateTimeKind.Utc);
        var evt = new ZendeskTicketImportedEvent(
            ticketId,
            externalId,
            title,
            description,
            url,
            mainCategory,
            category,
            status,
            country,
            merchantId,
            level3Team,
            tags,
            createdAt,
            updatedAt,
            [
                new ZendeskTicketImportedEvent.ZendeskTicketMessage(
                    messageId,
                    "msg-ext",
                    "Message content",
                    "Message author",
                    messageCreatedAt)
            ]);

        // act
        var ticket = ZendeskTicket.FromZendeskTicketImportedEvent(evt);

        // assert
        ticket.Id.Value.Should().Be(ticketId);
        ticket.ExternalId.Should().Be(externalId);
        ticket.Title.Should().Be(title);
        ticket.Description.Should().Be(description);
        ticket.Url.Should().Be(url);
        ticket.MainCategory.Should().Be(mainCategory);
        ticket.Category.Should().Be(category);
        ticket.Status.Should().Be(status);
        ticket.Country.Should().Be(country);
        ticket.MerchantId.Should().Be(merchantId);
        ticket.Level3Team.Should().Be(level3Team);
        ticket.Tags.Should().Equal(tags);
        ticket.CreatedAt.Should().Be(createdAt);
        ticket.UpdatedAt.Should().Be(updatedAt);
        ticket.Messages.Should().HaveCount(1);
        ticket.Messages[0].Id.Value.Should().Be(messageId);
        ticket.Messages[0].ExternalId.Should().Be("msg-ext");
        ticket.Messages[0].Content.Should().Be("Message content");
        ticket.Messages[0].Author.Should().Be("Message author");
        ticket.Messages[0].CreatedAt.Should().Be(messageCreatedAt);
    }

    [Fact]
    public void ToZendeskTicketImportedEvent_ReturnsEventWithTicketData()
    {
        // arrange
        var ticket = ZendeskTicket.Create(
            "to-event-ext",
            "To Event Title",
            "To Event description",
            "https://toevent.url",
            null,
            "open",
            null,
            null,
            [],
            DateTime.UtcNow,
            null,
            []);

        // act
        var evt = ticket.ToZendeskTicketImportedEvent();

        // assert
        evt.Id.Should().Be(ticket.Id.Value);
        evt.ExternalId.Should().Be(ticket.ExternalId);
        evt.Title.Should().Be(ticket.Title);
        evt.Description.Should().Be(ticket.Description);
        evt.Url.Should().Be(ticket.Url);
        evt.MainCategory.Should().Be(ticket.MainCategory);
        evt.Category.Should().Be(ticket.Category);
        evt.Status.Should().Be(ticket.Status);
        evt.Country.Should().Be(ticket.Country);
        evt.MerchantId.Should().Be(ticket.MerchantId);
        evt.Level3Team.Should().Be(ticket.Level3Team);
        evt.Tags.Should().Equal(ticket.Tags);
        evt.CreatedAt.Should().Be(ticket.CreatedAt);
        evt.UpdatedAt.Should().Be(ticket.UpdatedAt);
        evt.Messages.Should().HaveCount(ticket.Messages.Length);
    }

    [Fact]
    public void FromZendeskTicketImportedEvent_AndToZendeskTicketImportedEvent_RoundTrips()
    {
        // arrange
        var evt = new ZendeskTicketImportedEvent(
            Guid.NewGuid(),
            "roundtrip-ext",
            "Roundtrip Title",
            "Roundtrip description",
            "https://roundtrip.url",
            "MainCat",
            "MainCat__SubCat",
            "closed",
            "FI",
            "merchant-rt",
            "l3team",
            ["tag-a"],
            new DateTime(2025, 2, 1, 10, 0, 0, DateTimeKind.Utc),
            new DateTime(2025, 2, 1, 11, 0, 0, DateTimeKind.Utc),
            [
                new ZendeskTicketImportedEvent.ZendeskTicketMessage(
                    Guid.NewGuid(),
                    "msg-ext",
                    "Content",
                    "Author",
                    new DateTime(2025, 2, 1, 10, 30, 0, DateTimeKind.Utc))
            ]);

        // act
        var ticket = ZendeskTicket.FromZendeskTicketImportedEvent(evt);
        var roundTripped = ticket.ToZendeskTicketImportedEvent();

        // assert
        roundTripped.Id.Should().Be(evt.Id);
        roundTripped.ExternalId.Should().Be(evt.ExternalId);
        roundTripped.Title.Should().Be(evt.Title);
        roundTripped.Description.Should().Be(evt.Description);
        roundTripped.Url.Should().Be(evt.Url);
        roundTripped.MainCategory.Should().Be(evt.MainCategory);
        roundTripped.Category.Should().Be(evt.Category);
        roundTripped.Status.Should().Be(evt.Status);
        roundTripped.Country.Should().Be(evt.Country);
        roundTripped.MerchantId.Should().Be(evt.MerchantId);
        roundTripped.Level3Team.Should().Be(evt.Level3Team);
        roundTripped.Tags.Should().Equal(evt.Tags);
        roundTripped.CreatedAt.Should().Be(evt.CreatedAt);
        roundTripped.UpdatedAt.Should().Be(evt.UpdatedAt);
        roundTripped.Messages.Should().HaveCount(evt.Messages.Length);
        roundTripped.Messages[0].Id.Should().Be(evt.Messages[0].Id);
        roundTripped.Messages[0].ExternalId.Should().Be(evt.Messages[0].ExternalId);
        roundTripped.Messages[0].Content.Should().Be(evt.Messages[0].Content);
        roundTripped.Messages[0].Author.Should().Be(evt.Messages[0].Author);
        roundTripped.Messages[0].CreatedAt.Should().Be(evt.Messages[0].CreatedAt);
    }

    [Fact]
    public void IsRelevant_WhenSingleMessage_ReturnsFalse()
    {
        // arrange
        var ticket = ZendeskTicket.Create(
            "ext",
            "Normal Title",
            "Desc",
            "https://url",
            null,
            "open",
            null,
            null,
            [],
            DateTime.UtcNow,
            null,
            [
                new ZendeskTicket.ZendeskTicketMessage(
                    ZendeskTicketMessageId.New(),
                    "msg-1",
                    "Content",
                    "Author",
                    DateTime.UtcNow)
            ]);

        // act & assert
        ticket.IsRelevant.Should().BeFalse();
    }

    [Theory]
    [InlineData("closed")]
    [InlineData("solved")]
    public void IsRelevant_WhenTwoMessagesAndFinalStatus_ReturnsFalse(string status)
    {
        // arrange
        var ticket = ZendeskTicket.Create(
            "ext",
            "Normal Title",
            "Desc",
            "https://url",
            null,
            status,
            null,
            null,
            [],
            DateTime.UtcNow,
            null,
            [
                new ZendeskTicket.ZendeskTicketMessage(ZendeskTicketMessageId.New(), "m1", "C1", "A1", DateTime.UtcNow),
                new ZendeskTicket.ZendeskTicketMessage(ZendeskTicketMessageId.New(), "m2", "C2", "A2", DateTime.UtcNow)
            ]);

        // act & assert
        ticket.IsRelevant.Should().BeFalse();
    }

    [Theory]
    [InlineData("Incoming call")]
    [InlineData("Du har mottatt innsigelse")]
    [InlineData("Ni har fått en invändning")]
    [InlineData("Vi har fått en invändning")]
    public void IsRelevant_WhenTwoMessagesTitleStartsWith_ReturnsFalse(string title)
    {
        // arrange
        var ticket = ZendeskTicket.Create(
            "ext",
            $"{title} test",
            "Desc",
            "https://url",
            null,
            "open",
            null,
            null,
            [],
            DateTime.UtcNow,
            null,
            [
                new ZendeskTicket.ZendeskTicketMessage(ZendeskTicketMessageId.New(), "m1", "C1", "A1", DateTime.UtcNow),
                new ZendeskTicket.ZendeskTicketMessage(ZendeskTicketMessageId.New(), "m2", "C2", "A2", DateTime.UtcNow)
            ]);

        // act & assert
        ticket.IsRelevant.Should().BeFalse();
    }

    [Theory]
    [InlineData("Sinch call answered on")]
    [InlineData("Escalated dispute with KlarnaDisputeId")]
    public void IsRelevant_WhenTitleStartsWith_ReturnsFalse(string title)
    {
        // arrange
        var ticket = ZendeskTicket.Create(
            "ext",
            $"{title} test",
            "Desc",
            "https://url",
            null,
            "open",
            null,
            null,
            [],
            DateTime.UtcNow,
            null,
            [
                new ZendeskTicket.ZendeskTicketMessage(ZendeskTicketMessageId.New(), "m1", "C1", "A1", DateTime.UtcNow),
                new ZendeskTicket.ZendeskTicketMessage(ZendeskTicketMessageId.New(), "m2", "C2", "A2", DateTime.UtcNow),
                new ZendeskTicket.ZendeskTicketMessage(ZendeskTicketMessageId.New(), "m3", "C3", "A3", DateTime.UtcNow)
            ]);

        // act & assert
        ticket.IsRelevant.Should().BeFalse();
    }

    [Fact]
    public void IsRelevant_WhenMultipleMessagesAndNoneOfTheConditions_ReturnsTrue()
    {
        // arrange
        var ticket = ZendeskTicket.Create(
            "ext",
            "Normal support ticket title",
            "Desc",
            "https://url",
            null,
            "open",
            null,
            null,
            [],
            DateTime.UtcNow,
            null,
            [
                new ZendeskTicket.ZendeskTicketMessage(ZendeskTicketMessageId.New(), "m1", "C1", "A1", DateTime.UtcNow),
                new ZendeskTicket.ZendeskTicketMessage(ZendeskTicketMessageId.New(), "m2", "C2", "A2", DateTime.UtcNow),
                new ZendeskTicket.ZendeskTicketMessage(ZendeskTicketMessageId.New(), "m3", "C3", "A3", DateTime.UtcNow)
            ]);

        // act & assert
        ticket.IsRelevant.Should().BeTrue();
    }
}