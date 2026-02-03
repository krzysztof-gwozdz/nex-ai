using NexAI.Zendesk.Messages;

namespace NexAI.Zendesk.Tests;

public class ZendeskGroupTests
{
    [Fact]
    public void Constructor_AssignsAllProperties()
    {
        // arrange
        var id = ZendeskGroupId.New();
        const string externalId = "ext-123";
        const string name = "Support Team";

        // act
        var group = new ZendeskGroup(id, externalId, name);

        // assert
        group.Id.Should().Be(id);
        group.ExternalId.Should().Be(externalId);
        group.Name.Should().Be(name);
    }

    [Fact]
    public void Constructor_WithSameValues_ProducesEqualRecords()
    {
        // arrange
        var id = ZendeskGroupId.New();
        const string externalId = "same-ext";
        const string name = "Same Name";
        var group1 = new ZendeskGroup(id, externalId, name);
        var group2 = new ZendeskGroup(id, externalId, name);

        // act & assert
        group1.Should().Be(group2);
        (group1 == group2).Should().BeTrue();
    }

    [Fact]
    public void Create_ReturnsGroupWithGivenExternalIdAndName()
    {
        // arrange
        const string externalId = "create-ext-456";
        const string name = "Created Group";

        // act
        var group = ZendeskGroup.Create(externalId, name);

        // assert
        group.ExternalId.Should().Be(externalId);
        group.Name.Should().Be(name);
        group.Id.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_EachCall_GeneratesNewId()
    {
        // arrange & act
        var group1 = ZendeskGroup.Create("ext-1", "Name 1");
        var group2 = ZendeskGroup.Create("ext-2", "Name 2");

        // assert
        group1.Id.Value.Should().NotBe(group2.Id.Value);
    }

    [Fact]
    public void FromZendeskGroupImportedEvent_ReturnsGroupWithEventData()
    {
        // arrange
        var groupId = Guid.NewGuid();
        const string externalId = "event-ext-789";
        const string name = "Event Group";
        var evt = new ZendeskGroupImportedEvent(groupId, externalId, name);

        // act
        var group = ZendeskGroup.FromZendeskGroupImportedEvent(evt);

        // assert
        group.Id.Value.Should().Be(groupId);
        group.ExternalId.Should().Be(externalId);
        group.Name.Should().Be(name);
    }

    [Fact]
    public void ToZendeskGroupImportedEvent_ReturnsEventWithGroupData()
    {
        // arrange
        var group = ZendeskGroup.Create("to-event-ext", "To Event Group");

        // act
        var evt = group.ToZendeskGroupImportedEvent();

        // assert
        evt.Id.Should().Be(group.Id.Value);
        evt.ExternalId.Should().Be(group.ExternalId);
        evt.Name.Should().Be(group.Name);
    }
}
