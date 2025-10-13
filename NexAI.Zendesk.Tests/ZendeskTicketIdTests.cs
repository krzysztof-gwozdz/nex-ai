using FluentAssertions;
using Xunit;

namespace NexAI.Zendesk.Tests;

public class ZendeskTicketIdTests
{
    [Fact]
    public void Constructor_ForNotEmptyGuid_ConstructsNewId()
    {
        // arrange
        var guid = Guid.NewGuid();

        // act
        var zendeskTicketId = new ZendeskTicketId(guid);

        // assert
        zendeskTicketId.Value.Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_ForEmptyGuid_ThrowsException()
    {
        // arrange

        // act
        var constructor = () => new ZendeskTicketId(Guid.Empty);

        // assert
        constructor.Should().Throw<ArgumentException>()
            .WithMessage("Argument cannot be null or empty (Parameter 'value')");
    }

    [Fact]
    public void New_GeneratesNewId()
    {
        // arrange

        // act
        var zendeskTicketId = ZendeskTicketId.New();

        // assert
        zendeskTicketId.Value.Should().NotBeEmpty();
        zendeskTicketId.Value.Version.Should().Be(7);
    }

    [Fact]
    public void ToString_ReturnsStringRepresentationOfValue()
    {
        // arrange
        var guid = Guid.NewGuid();
        var zendeskTicketId = new ZendeskTicketId(guid);

        // act
        var value = zendeskTicketId.ToString();

        // assert
        value.Should().Be(guid.ToString());
    }

    [Fact]
    public void ToGuidOperator_ReturnsGuidRepresentationOfValue()
    {
        // arrange
        var guid = Guid.NewGuid();
        var zendeskTicketId = new ZendeskTicketId(guid);

        // act
        Guid value = zendeskTicketId;

        // assert
        value.Should().Be(guid);
    }

    [Fact]
    public void ToStringOperator_ReturnsStringRepresentationOfValue()
    {
        // arrange
        var guid = Guid.NewGuid();
        var zendeskTicketId = new ZendeskTicketId(guid);

        // value
        string value = zendeskTicketId;

        // assert
        value.Should().Be(guid.ToString());
    }
}