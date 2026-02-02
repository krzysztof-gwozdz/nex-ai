using FluentAssertions;
using NexAI.Zendesk.Messages;
using Xunit;

namespace NexAI.Zendesk.Tests;

public class ZendeskUserTests
{
    [Fact]
    public void Constructor_AssignsAllProperties()
    {
        // arrange
        var id = ZendeskUserId.New();
        const string externalId = "ext-123";
        const string name = "Jane Doe";
        const string email = "jane@example.com";

        // act
        var user = new ZendeskUser(id, externalId, name, email);

        // assert
        user.Id.Should().Be(id);
        user.ExternalId.Should().Be(externalId);
        user.Name.Should().Be(name);
        user.Email.Should().Be(email);
    }

    [Fact]
    public void Constructor_WithSameValues_ProducesEqualRecords()
    {
        // arrange
        var id = ZendeskUserId.New();
        const string externalId = "same-ext";
        const string name = "Same Name";
        const string email = "same@example.com";
        var user1 = new ZendeskUser(id, externalId, name, email);
        var user2 = new ZendeskUser(id, externalId, name, email);

        // act & assert
        user1.Should().Be(user2);
        (user1 == user2).Should().BeTrue();
    }

    [Fact]
    public void Create_ReturnsUserWithGivenExternalIdNameAndEmail()
    {
        // arrange
        const string externalId = "create-ext-456";
        const string name = "Created User";
        const string email = "created@example.com";

        // act
        var user = ZendeskUser.Create(externalId, name, email);

        // assert
        user.ExternalId.Should().Be(externalId);
        user.Name.Should().Be(name);
        user.Email.Should().Be(email);
        user.Id.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_EachCall_GeneratesNewId()
    {
        // arrange & act
        var user1 = ZendeskUser.Create("ext-1", "Name 1", "one@example.com");
        var user2 = ZendeskUser.Create("ext-2", "Name 2", "two@example.com");

        // assert
        user1.Id.Value.Should().NotBe(user2.Id.Value);
    }

    [Fact]
    public void FromZendeskUserImportedEvent_ReturnsUserWithEventData()
    {
        // arrange
        var userId = Guid.NewGuid();
        const string externalId = "event-ext-789";
        const string name = "Event User";
        const string email = "event@example.com";
        var evt = new ZendeskUserImportedEvent(userId, externalId, name, email);

        // act
        var user = ZendeskUser.FromZendeskUserImportedEvent(evt);

        // assert
        user.Id.Value.Should().Be(userId);
        user.ExternalId.Should().Be(externalId);
        user.Name.Should().Be(name);
        user.Email.Should().Be(email);
    }

    [Fact]
    public void ToZendeskUserImportedEvent_ReturnsEventWithUserData()
    {
        // arrange
        var user = ZendeskUser.Create("to-event-ext", "To Event User", "toevent@example.com");

        // act
        var evt = user.ToZendeskUserImportedEvent();

        // assert
        evt.Id.Should().Be(user.Id.Value);
        evt.ExternalId.Should().Be(user.ExternalId);
        evt.Name.Should().Be(user.Name);
        evt.Email.Should().Be(user.Email);
    }
}
