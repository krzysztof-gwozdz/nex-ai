using FluentAssertions;
using NexAI.Zendesk.Api.Dtos;
using Xunit;

namespace NexAI.Zendesk.Tests;

public class ZendeskUserMapperTests
{
    [Fact]
    public void Map_GeneratesNewId()
    {
        // arrange
        var user = ValidUserDto;

        // act
        var result = ZendeskUserMapper.Map(user);

        // assert
        result.Id.Value.Should().NotBeEmpty();
        result.Id.Value.Version.Should().Be(7);
    }

    [Fact]
    public void Map_WithValidId_SetsExternalIdToStringValue()
    {
        // arrange
        var user = ValidUserDto with { Id = 12345L };

        // act
        var result = ZendeskUserMapper.Map(user);

        // assert
        result.ExternalId.Should().Be(user.Id!.Value.ToString());
    }

    [Fact]
    public void Map_WithNullId_ThrowsException()
    {
        // arrange
        var user = ValidUserDto with { Id = null };

        // act
        var map = () => ZendeskUserMapper.Map(user);

        // assert
        map.Should().Throw<Exception>().WithMessage("Could not parse Id");
    }

    [Fact]
    public void Map_WithNegativeId_ThrowsException()
    {
        // arrange
        var user = ValidUserDto with { Id = -1 };

        // act
        var map = () => ZendeskUserMapper.Map(user);

        // assert
        map.Should().Throw<Exception>().WithMessage("Could not parse Id");
    }

    [Fact]
    public void Map_WithValidSubject_SetsNameToNormalizedValue()
    {
        // arrange
        var user = ValidUserDto with { Name = "Test User" };

        // act
        var result = ZendeskUserMapper.Map(user);

        // assert
        result.Name.Should().Be("Test User");
    }

    [Fact]
    public void Map_WithNullSubject_SetsMissingNamePlaceholder()
    {
        // arrange
        var user = ValidUserDto with { Name = null };

        // act
        var result = ZendeskUserMapper.Map(user);

        // assert
        result.Name.Should().Be("<MISSING NAME>");
    }

    [Fact]
    public void Map_WithWhitespaceSubject_SetsMissingNamePlaceholder()
    {
        // arrange
        var user = ValidUserDto with { Name = " \n\t  " };

        // act
        var result = ZendeskUserMapper.Map(user);

        // asserts
        result.Name.Should().Be("<MISSING NAME>");
    }

    private static UserDto ValidUserDto =>
        new(
            Url: null,
            Id: 100,
            Name: "Employee",
            Email: null,
            CreatedAt: "1970-01-01T00:00:00Z",
            UpdatedAt: null,
            TimeZone: null,
            IanaTimeZone: null,
            Phone: null,
            SharedPhoneNumber: null,
            Photo: null,
            RemotePhotoUrl: null,
            ChatOnly: null,
            LocaleId: null,
            Locale: null,
            OrganizationId: null,
            Role: null,
            Verified: null,
            ExternalId: null,
            Tags: null,
            Alias: null,
            Active: null,
            Shared: null,
            SharedAgent: null,
            LastLoginAt: null,
            TwoFactorAuthEnabled: null,
            Signature: null,
            Details: null,
            Notes: null,
            RoleType: null,
            CustomRoleId: null,
            Moderator: null,
            TicketRestriction: null,
            OnlyPrivateComments: null,
            RestrictedAgent: null,
            Suspended: null,
            DefaultGroupId: null,
            ReportCsv: null,
            UserFields: null
        );
}