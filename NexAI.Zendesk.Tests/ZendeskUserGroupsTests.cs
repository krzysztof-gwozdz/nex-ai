using FluentAssertions;
using NexAI.Zendesk.Messages;
using Xunit;

namespace NexAI.Zendesk.Tests;

public class ZendeskUserGroupsTests
{
    [Fact]
    public void Constructor_AssignsAllProperties()
    {
        // arrange
        var userId = ZendeskUserId.New();
        var groups = new[] { ZendeskGroupId.New(), ZendeskGroupId.New() };

        // act
        var userGroups = new ZendeskUserGroups(userId, groups);

        // assert
        userGroups.UserId.Should().Be(userId);
        userGroups.Groups.Should().BeSameAs(groups);
    }

    [Fact]
    public void Constructor_WithSameValues_ProducesEqualRecords()
    {
        // arrange
        var userId = ZendeskUserId.New();
        var groups = new[] { ZendeskGroupId.New(), ZendeskGroupId.New() };
        var userGroups1 = new ZendeskUserGroups(userId, groups);
        var userGroups2 = new ZendeskUserGroups(userId, groups);

        // act & assert
        userGroups1.Should().Be(userGroups2);
        (userGroups1 == userGroups2).Should().BeTrue();
    }

    [Fact]
    public void FromZendeskUserGroupsImportedEvent_ReturnsUserGroupsWithEventData()
    {
        // arrange
        var userId = Guid.NewGuid();
        var groupIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var evt = new ZendeskUserGroupsImportedEvent(userId, groupIds);

        // act
        var userGroups = ZendeskUserGroups.FromZendeskUserGroupsImportedEvent(evt);

        // assert
        userGroups.UserId.Value.Should().Be(userId);
        userGroups.Groups.Should().HaveCount(2);
        userGroups.Groups[0].Value.Should().Be(groupIds[0]);
        userGroups.Groups[1].Value.Should().Be(groupIds[1]);
    }

    [Fact]
    public void ToZendeskUserGroupsImportedEvent_ReturnsEventWithUserGroupsData()
    {
        // arrange
        var userId = ZendeskUserId.New();
        var groups = new[] { ZendeskGroupId.New(), ZendeskGroupId.New() };
        var userGroups = new ZendeskUserGroups(userId, groups);

        // act
        var evt = userGroups.ToZendeskUserGroupsImportedEvent();

        // assert
        evt.UserId.Should().Be(userGroups.UserId.Value);
        evt.Groups.Should().HaveCount(2);
        evt.Groups[0].Should().Be(userGroups.Groups[0].Value);
        evt.Groups[1].Should().Be(userGroups.Groups[1].Value);
    }
}
