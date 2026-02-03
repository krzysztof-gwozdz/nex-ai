namespace NexAI.Zendesk.Tests;

public class ZendeskUserIdTests
{
    [Fact]
    public void Constructor_WithValidGuid_AssignsValue()
    {
        // arrange
        var guid = Guid.NewGuid();

        // act
        var id = new ZendeskUserId(guid);

        // assert
        id.Value.Should().Be(guid);
    }

    [Fact]
    public void Constructor_WithEmptyGuid_Throws()
    {
        // arrange
        var act = () => new ZendeskUserId(Guid.Empty);

        // act & assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("value");
    }

    [Fact]
    public void New_ReturnsIdWithNonEmptyGuid()
    {
        // act
        var id = ZendeskUserId.New();

        // assert
        id.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void New_EachCall_GeneratesDifferentId()
    {
        // arrange & act
        var id1 = ZendeskUserId.New();
        var id2 = ZendeskUserId.New();

        // assert
        id1.Value.Should().NotBe(id2.Value);
    }

    [Fact]
    public void ToString_ReturnsValueToString()
    {
        // arrange
        var guid = Guid.NewGuid();
        var id = new ZendeskUserId(guid);

        // act
        var result = id.ToString();

        // assert
        result.Should().Be(guid.ToString());
    }

    [Fact]
    public void ImplicitConversionToGuid_ReturnsValue()
    {
        // arrange
        var guid = Guid.NewGuid();
        var id = new ZendeskUserId(guid);

        // act
        Guid result = id;

        // assert
        result.Should().Be(guid);
    }

    [Fact]
    public void ImplicitConversionToString_ReturnsValueToString()
    {
        // arrange
        var guid = Guid.NewGuid();
        var id = new ZendeskUserId(guid);

        // act
        string result = id;

        // assert
        result.Should().Be(guid.ToString());
    }

    [Fact]
    public void Equal_SameValue_ProducesEqualRecords()
    {
        // arrange
        var guid = Guid.NewGuid();
        var id1 = new ZendeskUserId(guid);
        var id2 = new ZendeskUserId(guid);

        // act & assert
        id1.Should().Be(id2);
        (id1 == id2).Should().BeTrue();
    }

    [Fact]
    public void Equal_DifferentValues_ProducesEqualRecords()
    {
        // arrange
        var id1 = new ZendeskUserId(Guid.NewGuid());
        var id2 = new ZendeskUserId(Guid.NewGuid());

        // act & assert
        id1.Should().NotBe(id2);
        (id1 == id2).Should().BeFalse();
    }
}
