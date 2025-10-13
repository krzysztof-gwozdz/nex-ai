using FluentAssertions;
using Xunit;

namespace NexAI.Zendesk.Tests;

public class ZendeskGroupIdTests
{
    [Fact]
    public void Constructor_ForNotEmptyGuid_ConstructsNewId()
    {
        // arrange
        var guid = Guid.NewGuid();

        // act
        var zendeskGroupId = new ZendeskGroupId(guid);

        // assert
        zendeskGroupId.Value.Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_ForEmptyGuid_ThrowsException()
    {
        // arrange

        // act
        var constructor = () => new ZendeskGroupId(Guid.Empty);

        // assert
        constructor.Should().Throw<ArgumentException>()
            .WithMessage("Argument cannot be null or empty (Parameter 'value')");
    }

    [Fact]
    public void New_GeneratesNewId()
    {
        // arrange

        // act
        var zendeskGroupId = ZendeskGroupId.New();

        // assert
        zendeskGroupId.Value.Should().NotBeEmpty();
        zendeskGroupId.Value.Version.Should().Be(7);
    }

    [Fact]
    public void ToString_ReturnsStringRepresentationOfValue()
    {
        // arrange
        var guid = Guid.NewGuid();
        var zendeskGroupId = new ZendeskGroupId(guid);

        // act
        var value = zendeskGroupId.ToString();

        // assert
        value.Should().Be(guid.ToString());
    }

    [Fact]
    public void ToGuidOperator_ReturnsGuidRepresentationOfValue()
    {
        // arrange
        var guid = Guid.NewGuid();
        var zendeskGroupId = new ZendeskGroupId(guid);

        // act
        Guid value = zendeskGroupId;

        // assert
        value.Should().Be(guid);
    }

    [Fact]
    public void ToStringOperator_ReturnsStringRepresentationOfValue()
    {
        // arrange
        var guid = Guid.NewGuid();
        var zendeskGroupId = new ZendeskGroupId(guid);

        // value
        string value = zendeskGroupId;

        // assert
        value.Should().Be(guid.ToString());
    }
}