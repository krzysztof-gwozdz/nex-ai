using FluentAssertions;
using NexAI.Zendesk.Api.Dtos;
using Xunit;

namespace NexAI.Zendesk.Tests;

public class ZendeskGroupMapperTests
{
    [Fact]
    public void Map_GeneratesNewId()
    {
        // arrange
        var group = ValidGroupDto;

        // act
        var result = ZendeskGroupMapper.Map(group);

        // assert
        result.Id.Value.Should().NotBeEmpty();
        result.Id.Value.Version.Should().Be(7);
    }

    [Fact]
    public void Map_WithValidId_SetsExternalIdToStringValue()
    {
        // arrange
        var group = ValidGroupDto with { Id = 12345L };

        // act
        var result = ZendeskGroupMapper.Map(group);

        // assert
        result.ExternalId.Should().Be(group.Id!.Value.ToString());
    }

    [Fact]
    public void Map_WithNullId_ThrowsException()
    {
        // arrange
        var group = ValidGroupDto with { Id = null };

        // act
        var map = () => ZendeskGroupMapper.Map(group);

        // assert
        map.Should().Throw<Exception>().WithMessage("Could not parse Id");
    }

    [Fact]
    public void Map_WithNegativeId_ThrowsException()
    {
        // arrange
        var group = ValidGroupDto with { Id = -1 };

        // act
        var map = () => ZendeskGroupMapper.Map(group);

        // assert
        map.Should().Throw<Exception>().WithMessage("Could not parse Id");
    }

    [Fact]
    public void Map_WithValidSubject_SetsNameToNormalizedValue()
    {
        // arrange
        var group = ValidGroupDto with { Name = "Test Group" };

        // act
        var result = ZendeskGroupMapper.Map(group);

        // assert
        result.Name.Should().Be("Test Group");
    }

    [Fact]
    public void Map_WithNullSubject_SetsMissingNamePlaceholder()
    {
        // arrange
        var group = ValidGroupDto with { Name = null };

        // act
        var result = ZendeskGroupMapper.Map(group);

        // assert
        result.Name.Should().Be("<MISSING NAME>");
    }

    [Fact]
    public void Map_WithWhitespaceSubject_SetsMissingNamePlaceholder()
    {
        // arrange
        var group = ValidGroupDto with { Name = " \n\t  " };

        // act
        var result = ZendeskGroupMapper.Map(group);

        // asserts
        result.Name.Should().Be("<MISSING NAME>");
    }

    private static GroupDto ValidGroupDto =>
        new(
            Url: null,
            Id: 100,
            IsPublic: true,
            Name: "Admins",
            Description: "Group for all admin users",
            Default: false,
            Deleted: false,
            CreatedAt: "1970-01-01T00:00:00Z",
            UpdatedAt: null
        );
}