using FluentAssertions;
using Xunit;

namespace NexAI.Zendesk.Tests;

public class ZendeskUserIdTests
{
    [Fact]
    public void Constructor_ForNotEmptyGuid_ConstructsNewId()
    {
        // arrange
        var guid = Guid.NewGuid();

        // act
        var zendeskUserId = new ZendeskUserId(guid);

        // assert
        zendeskUserId.Value.Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_ForEmptyGuid_ThrowsException()
    {
        // arrange

        // act
        var constructor = () => new ZendeskUserId(Guid.Empty);

        // assert
        constructor.Should().Throw<ArgumentException>()
            .WithMessage("Argument cannot be null or empty (Parameter 'value')");
    }

    [Fact]
    public void New_GeneratesNewId()
    {
        // arrange

        // act
        var zendeskUserId = ZendeskUserId.New();

        // assert
        zendeskUserId.Value.Should().NotBeEmpty();
        zendeskUserId.Value.Version.Should().Be(7);
    }

    [Fact]
    public void ToString_ReturnsStringRepresentationOfValue()
    {
        // arrange
        var guid = Guid.NewGuid();
        var zendeskUserId = new ZendeskUserId(guid);

        // act
        var value = zendeskUserId.ToString();

        // assert
        value.Should().Be(guid.ToString());
    }

    [Fact]
    public void ToGuidOperator_ReturnsGuidRepresentationOfValue()
    {
        // arrange
        var guid = Guid.NewGuid();
        var zendeskUserId = new ZendeskUserId(guid);

        // act
        Guid value = zendeskUserId;

        // assert
        value.Should().Be(guid);
    }

    [Fact]
    public void ToStringOperator_ReturnsStringRepresentationOfValue()
    {
        // arrange
        var guid = Guid.NewGuid();
        var zendeskUserId = new ZendeskUserId(guid);

        // value
        string value = zendeskUserId;

        // assert
        value.Should().Be(guid.ToString());
    }
}