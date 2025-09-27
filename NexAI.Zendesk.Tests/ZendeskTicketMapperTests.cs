using FluentAssertions;
using NexAI.Zendesk.Api.Dtos;
using Xunit;

namespace NexAI.Zendesk.Tests;

public class ZendeskTicketMapperTests
{
    [Fact]
    public void Map_GeneratesNewId()
    {
        // arrange
        var ticket = ValidTicketDto;

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Id.Value.Should().NotBeEmpty();
        result.Id.Value.Version.Should().Be(7);
    }

    [Fact]
    public void Map_WithValidId_SetsExternalIdToStringValue()
    {
        // arrange
        var ticket = ValidTicketDto with { Id = 12345L };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.ExternalId.Should().Be(ticket.Id!.Value.ToString());
    }

    [Fact]
    public void Map_WithNullId_ThrowsException()
    {
        // arrange
        var ticket = ValidTicketDto with { Id = null };

        // act
        var map = () => ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        map.Should().Throw<Exception>().WithMessage("Could not parse Id");
    }

    [Fact]
    public void Map_WithNegativeId_ThrowsException()
    {
        // arrange
        var ticket = ValidTicketDto with { Id = -1 };

        // act
        var map = () => ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        map.Should().Throw<Exception>().WithMessage("Could not parse Id");
    }

    [Fact]
    public void Map_WithValidSubject_SetsTitleToNormalizedValue()
    {
        // arrange
        var ticket = ValidTicketDto with { Subject = "Test Ticket" };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Title.Should().Be("Test Ticket");
    }

    [Fact]
    public void Map_WithNullSubject_SetsMissingTitlePlaceholder()
    {
        // arrange
        var ticket = ValidTicketDto with { Subject = null };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Title.Should().Be("<MISSING TITLE>");
    }

    [Fact]
    public void Map_WithWhitespaceSubject_SetsMissingTitlePlaceholder()
    {
        // arrange
        var ticket = ValidTicketDto with { Subject = "\u200B  \n\t  " };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Title.Should().Be("<MISSING TITLE>");
    }

    [Fact]
    public void Map_WithZerosWidthCharsSubject_SetsMissingTitlePlaceholder()
    {
        // arrange
        var ticket = ValidTicketDto with { Subject = "\u200B\u200C\u200D\uFEFF " };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Title.Should().Be("<MISSING TITLE>");
    }

    [Fact]
    public void Map_WithValidDescription_SetsDescriptionToNormalizedValue()
    {
        // arrange
        var ticket = ValidTicketDto with { Description = "Test Ticket" };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Description.Should().Be("Test Ticket");
    }

    [Fact]
    public void Map_WithNullDescription_SetsMissingDescriptionPlaceholder()
    {
        // arrange
        var ticket = ValidTicketDto with { Description = null };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Description.Should().Be("<MISSING DESCRIPTION>");
    }

    [Fact]
    public void Map_WithWhitespaceDescription_SetsMissingDescriptionPlaceholder()
    {
        // arrange
        var ticket = ValidTicketDto with { Description = "\u200B  \n\t  " };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Description.Should().Be("<MISSING DESCRIPTION>");
    }

    [Fact]
    public void Map_WithZerosWidthCharsDescription_SetsMissingDescriptionPlaceholder()
    {
        // arrange
        var ticket = ValidTicketDto with { Description = "\u200B\u200C\u200D\uFEFF " };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Description.Should().Be("<MISSING DESCRIPTION>");
    }

    [Theory]
    [InlineData("john.doe@example.com")]
    [InlineData("support+test@my-domain.org")]
    [InlineData("user_name123@sub.mail.co.uk")]
    [InlineData("admin@mail-server.io")]
    [InlineData("hello.world@foo.bar")]
    [InlineData("first.last@iana.org")]
    [InlineData("contact@company.travel")]
    [InlineData("dev_team42@startup.tech")]
    public void Map_WithDescriptionThatContainsEmails_MaskEmails(string description)
    {
        // arrange
        var ticket = ValidTicketDto with { Description = description };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Description.Should().Be("<EMAIL ADDRESS>");
    }

    [Theory]
    [InlineData("+1 202-555-0173")]
    [InlineData("+44 20 7946 0958")]
    [InlineData("202.555.0199")]
    [InlineData("(202) 555-0147")]
    [InlineData("0031 20 123 4567")]
    [InlineData("+49 (30) 901820")]
    [InlineData("020-7946-0958")]
    [InlineData("2025550182")]
    public void Map_WithDescriptionThatContainsPhoneNumbers_MaskEmails(string description)
    {
        // arrange
        var ticket = ValidTicketDto with { Description = description };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Description.Should().Be("<PHONE NUMBER>");
    }

    [Theory]
    [InlineData("![Logo](https://example.com/logo.png)")]
    [InlineData("![Photo](http://images.example.org/photo.jpg)")]
    [InlineData("![Diagram](https://cdn.example.net/diagram.svg)")]
    [InlineData("![Profile Pic](https://example.com/users/john/avatar.jpeg)")]
    [InlineData("![Chart](http://example.com/data/chart.gif)")]
    [InlineData("![Screenshot](https://sub.domain.com/path/to/screenshot.webp)")]
    [InlineData("![Banner](https://example.org/images/banner.PNG)")]
    [InlineData("![Icon](http://static.example.com/icons/icon.ico)")]
    public void Map_WithDescriptionThatContainsImageUrls_MaskEmails(string description)
    {
        // arrange
        var ticket = ValidTicketDto with { Description = description };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Description.Should().Be("<IMAGE URL>");
    }

    [Fact]
    public void Map_WithValidUrl_SetsUrlToNormalizedValue()
    {
        // arrange
        var ticket = ValidTicketDto with { Url = "www.test.com" };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Url.Should().Be("www.test.com");
    }

    [Fact]
    public void Map_WithNullUrl_SetsMissingUrlPlaceholder()
    {
        // arrange
        var ticket = ValidTicketDto with { Url = null };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Url.Should().Be("<MISSING URL>");
    }

    [Fact]
    public void Map_WithWhitespaceUrl_SetsMissingUrlPlaceholder()
    {
        // arrange
        var ticket = ValidTicketDto with { Url = "  \n\t  " };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Url.Should().Be("<MISSING URL>");
    }

    [Fact]
    public void Map_WithCustomFieldsThatContainsCategory_SetsCategory()
    {
        // arrange
        var ticket = ValidTicketDto with { CustomFields = [new(23426028, "test category")] };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Category.Should().Be("test category");
    }

    [Fact]
    public void Map_WithNullCustomFields_SetsMissingCategoryPlaceholder()
    {
        // arrange
        var ticket = ValidTicketDto with { CustomFields = null };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Category.Should().BeNull();
    }

    [Fact]
    public void Map_WithCustomFieldsWithoutCategory_SetsMissingCategoryPlaceholder()
    {
        // arrange
        var ticket = ValidTicketDto with { CustomFields = [new(-1, "test field")] };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Category.Should().BeNull();
    }

    [Fact]
    public void Map_WithCustomFieldsThatContainsCategoryWithNullValue_SetsCategory()
    {
        // arrange
        var ticket = ValidTicketDto with { CustomFields = [new(23426028, null)] };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Category.Should().BeNull();
    }

    [Fact]
    public void Map_WithCustomFieldsThatContainsCategoryWithEmptyValue_SetsCategory()
    {
        // arrange
        var ticket = ValidTicketDto with { CustomFields = [new(23426028, "  \n\t  ")] };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Category.Should().BeNull();
    }

    [Fact]
    public void Map_WithValidStatus_SetsStatusToNormalizedValue()
    {
        // arrange
        var ticket = ValidTicketDto with { Status = "open" };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Status.Should().Be("open");
    }

    [Fact]
    public void Map_WithNullStatus_SetsMissingStatusPlaceholder()
    {
        // arrange
        var ticket = ValidTicketDto with { Status = null };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Status.Should().Be("<MISSING STATUS>");
    }

    [Fact]
    public void Map_WithWhitespaceStatus_SetsMissingStatusPlaceholder()
    {
        // arrange
        var ticket = ValidTicketDto with { Status = "  \n\t  " };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Status.Should().Be("<MISSING STATUS>");
    }

    [Fact]
    public void Map_WithCustomFieldsThatContainsCountry_SetsCountry()
    {
        // arrange
        var ticket = ValidTicketDto with { CustomFields = [new(360000060007, "Netherlands")] };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Country.Should().Be("Netherlands");
    }

    [Fact]
    public void Map_WithNullCustomFields_SetsMissingCountryPlaceholder()
    {
        // arrange
        var ticket = ValidTicketDto with { CustomFields = null };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Country.Should().BeNull();
    }

    [Fact]
    public void Map_WithCustomFieldsWithoutCountry_SetsMissingCountryPlaceholder()
    {
        // arrange
        var ticket = ValidTicketDto with { CustomFields = [new(-1, "some field")] };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Country.Should().BeNull();
    }

    [Fact]
    public void Map_WithCustomFieldsThatContainsCountryWithNullValue_SetsMissingCountryPlaceholder()
    {
        // arrange
        var ticket = ValidTicketDto with { CustomFields = [new(360000060007, null)] };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Country.Should().BeNull();
    }

    [Fact]
    public void Map_WithCustomFieldsThatContainsCountryWithEmptyValue_SetsMissingCountryPlaceholder()
    {
        // arrange
        var ticket = ValidTicketDto with { CustomFields = [new(360000060007, "  \n\t  ")] };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Country.Should().BeNull();
    }

    [Fact]
    public void Map_WithCustomFieldsThatContainsMerchantId_SetsMerchantId()
    {
        // arrange
        var ticket = ValidTicketDto with { CustomFields = [new(21072413, "merchant-123")] };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.MerchantId.Should().Be("merchant-123");
    }

    [Fact]
    public void Map_WithNullCustomFields_SetsMissingMerchantIdPlaceholder()
    {
        // arrange
        var ticket = ValidTicketDto with { CustomFields = null };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.MerchantId.Should().BeNull();
    }

    [Fact]
    public void Map_WithCustomFieldsWithoutMerchantId_SetsMissingMerchantIdPlaceholder()
    {
        // arrange
        var ticket = ValidTicketDto with { CustomFields = [new(-1, "some field")] };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.MerchantId.Should().BeNull();
    }

    [Fact]
    public void Map_WithCustomFieldsThatContainsMerchantIdWithNullValue_SetsMissingMerchantIdPlaceholder()
    {
        // arrange
        var ticket = ValidTicketDto with { CustomFields = [new(21072413, null)] };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.MerchantId.Should().BeNull();
    }

    [Fact]
    public void Map_WithCustomFieldsThatContainsMerchantIdWithEmptyValue_SetsMissingMerchantIdPlaceholder()
    {
        // arrange
        var ticket = ValidTicketDto with { CustomFields = [new(21072413, "  \n\t  ")] };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.MerchantId.Should().BeNull();
    }

    [Theory]
    [InlineData("0")]
    [InlineData("00")]
    [InlineData("1")]
    public void Map_WithCustomFieldsThatContainsMerchantIdWithInvalidValue_SetsMissingMerchantIdPlaceholder(string? merchantId)
    {
        // arrange
        var ticket = ValidTicketDto with { CustomFields = [new(21072413, merchantId)] };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.MerchantId.Should().BeNull();
    }

    [Fact]
    public void Map_WithTags_SetsTags()
    {
        // arrange
        var ticket = ValidTicketDto with { Tags = ["tag1", "tag2"] };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Tags.Should().HaveCount(ticket.Tags!.Length).And.ContainInOrder(ticket.Tags);
    }

    [Fact]
    public void Map_WithNullTags_SetsTagsToEmptyList()
    {
        // arrange
        var ticket = ValidTicketDto with { Tags = null };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.Tags.Should().BeEmpty();
    }

    [Fact]
    public void Map_WithValidCreatedAt_SetsCreatedAtToParsedValue()
    {
        // arrange
        const string iso = "2024-05-01T12:34:56Z";
        var ticket = ValidTicketDto with { CreatedAt = iso };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.CreatedAt.Should().Be(DateTime.Parse(iso));
    }

    [Fact]
    public void Map_WithNullCreatedAt_ThrowsException()
    {
        // arrange
        var ticket = ValidTicketDto with { CreatedAt = null };

        // act
        var map = () => ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        map.Should().Throw<Exception>().WithMessage("Could not parse Created At");
    }

    [Fact]
    public void Map_WithInvalidCreatedAt_ThrowsException()
    {
        // arrange
        var ticket = ValidTicketDto with { CreatedAt = "not-a-date" };

        // act
        var map = () => ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        map.Should().Throw<Exception>().WithMessage("Could not parse Created At");
    }

    [Fact]
    public void Map_WithValidUpdatedAt_SetsUpdatedAtToParsedValue()
    {
        // arrange
        const string iso = "2024-06-02T01:02:03Z";
        var ticket = ValidTicketDto with { UpdatedAt = iso };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.UpdatedAt.Should().Be(DateTime.Parse(iso));
    }

    [Fact]
    public void Map_WithNullUpdatedAt_SetsUpdatedAtToNull()
    {
        // arrange
        var ticket = ValidTicketDto with { UpdatedAt = null };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        result.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void Map_WithInvalidUpdatedAt_ThrowsException()
    {
        // arrange
        var ticket = ValidTicketDto with { UpdatedAt = "invalid" };

        // act
        var map = () => ZendeskTicketMapper.Map(ticket, [], []);

        // assert
        map.Should().Throw<Exception>().WithMessage("Could not Updated At");
    }

    [Fact]
    public void Map_WithValidComments_SetsMessages()
    {
        // arrange
        var ticket = ValidTicketDto;
        var comments = new[] { ValidCommentDto };
        var employees = new[] { ValidUserDto };

        // act
        var result = ZendeskTicketMapper.Map(ticket, comments, employees);

        // assert
        result.Messages.Should().ContainSingle();
        result.Messages[0].Author.Should().Be(employees[0].Name);
        result.Messages[0].Content.Should().Be(comments[0].PlainBody);
        result.Messages[0].CreatedAt.Should().Be(DateTime.Parse(comments[0].CreatedAt!));
    }

    [Fact]
    public void Map_WithValidComments_GeneratesNewMessageId()
    {
        // arrange
        var ticket = ValidTicketDto;
        var comments = new[] { ValidCommentDto };
        var employees = new[] { ValidUserDto };

        // act
        var result = ZendeskTicketMapper.Map(ticket, comments, employees);

        // assert
        result.Messages.Should().ContainSingle();
        result.Messages[0].Id.Value.Should().NotBeEmpty();
        result.Messages[0].Id.Value.Version.Should().Be(7);
    }

    [Fact]
    public void Map_WithValidCommentsWithValidId_SetsMessageExternalIdToStringValue()
    {
        // arrange
        var ticket = ValidTicketDto;
        var comments = new[] { ValidCommentDto };
        var employees = new[] { ValidUserDto };

        // act
        var result = ZendeskTicketMapper.Map(ticket, comments, employees);

        // assert
        result.Messages.Should().ContainSingle();
        result.Messages[0].ExternalId.Should().Be(comments[0].Id.ToString());
    }

    [Fact]
    public void Map_WithValidCommentsWithNullId_ThrowsException()
    {
        // arrange
        var ticket = ValidTicketDto;
        var comments = new[] { ValidCommentDto with { Id = null } };
        var employees = new[] { ValidUserDto };

        // act
        var map = () => ZendeskTicketMapper.Map(ticket, comments, employees);

        // assert
        map.Should().Throw<Exception>().WithMessage("Could not parse Id");
    }

    [Fact]
    public void Map_WithValidCommentsWithNegativeId_ThrowsException()
    {
        // arrange
        var ticket = ValidTicketDto;
        var comments = new[] { ValidCommentDto with { Id = -1 } };
        var employees = new[] { ValidUserDto };

        // act
        var map = () => ZendeskTicketMapper.Map(ticket, comments, employees);

        // assert
        map.Should().Throw<Exception>().WithMessage("Could not parse Id");
    }

    [Fact]
    public void Map_WithoutComments_SetsMissingCommentPlaceholder()
    {
        // arrange
        var ticket = ValidTicketDto;
        var employees = new[] { ValidUserDto };

        // act
        var result = ZendeskTicketMapper.Map(ticket, [], employees);

        // assert
        result.Messages.Should().BeEmpty();
    }

    [Fact]
    public void Map_WithNullCommentBody_SetsMissingCommentPlaceholder()
    {
        // arrange
        var ticket = ValidTicketDto;
        var comments = new[] { ValidCommentDto with { PlainBody = null } };
        var employees = new[] { ValidUserDto };

        // act
        var result = ZendeskTicketMapper.Map(ticket, comments, employees);

        // assert
        result.Messages[0].Content.Should().BeEmpty();
    }

    [Fact]
    public void Map_WithWhitespaceCommentBody_SetsMissingCommentPlaceholder()
    {
        // arrange
        var ticket = ValidTicketDto;
        var comments = new[] { ValidCommentDto with { PlainBody = "\u200B  \n\t  " } };
        var employees = new[] { ValidUserDto };

        // act
        var result = ZendeskTicketMapper.Map(ticket, comments, employees);

        // assert
        result.Messages[0].Content.Should().BeEmpty();
    }

    [Fact]
    public void Map_WithZerosWidthCharsCommentBody_SetsMissingCommentPlaceholder()
    {
        // arrange
        var ticket = ValidTicketDto;
        var comments = new[] { ValidCommentDto with { PlainBody = "\u200B\u200C\u200D\uFEFF " } };
        var employees = new[] { ValidUserDto };

        // act
        var result = ZendeskTicketMapper.Map(ticket, comments, employees);

        // assert
        result.Messages[0].Content.Should().BeEmpty();
    }

    [Theory]
    [InlineData("john.doe@example.com")]
    [InlineData("support+test@my-domain.org")]
    [InlineData("user_name123@sub.mail.co.uk")]
    [InlineData("admin@mail-server.io")]
    [InlineData("hello.world@foo.bar")]
    [InlineData("first.last@iana.org")]
    [InlineData("contact@company.travel")]
    [InlineData("dev_team42@startup.tech")]
    public void Map_WithCommentBodyThatContainsEmails_MaskEmails(string commentBody)
    {
        // arrange
        var ticket = ValidTicketDto;
        var comments = new[] { ValidCommentDto with { PlainBody = commentBody } };
        var employees = new[] { ValidUserDto };

        // act
        var result = ZendeskTicketMapper.Map(ticket, comments, employees);

        // assert
        result.Messages[0].Content.Should().Be("<EMAIL ADDRESS>");
    }

    [Theory]
    [InlineData("+1 202-555-0173")]
    [InlineData("+44 20 7946 0958")]
    [InlineData("202.555.0199")]
    [InlineData("(202) 555-0147")]
    [InlineData("0031 20 123 4567")]
    [InlineData("+49 (30) 901820")]
    [InlineData("020-7946-0958")]
    [InlineData("2025550182")]
    public void Map_WithCommentBodyThatContainsPhoneNumbers_MaskPhoneNumbers(string commentBody)
    {
        // arrange
        var ticket = ValidTicketDto;
        var comments = new[] { ValidCommentDto with { PlainBody = commentBody } };
        var employees = new[] { ValidUserDto };

        // act
        var result = ZendeskTicketMapper.Map(ticket, comments, employees);

        // assert
        result.Messages[0].Content.Should().Be("<PHONE NUMBER>");
    }

    [Theory]
    [InlineData("![Logo](https://example.com/logo.png)")]
    [InlineData("![Photo](http://images.example.org/photo.jpg)")]
    [InlineData("![Diagram](https://cdn.example.net/diagram.svg)")]
    [InlineData("![Profile Pic](https://example.com/users/john/avatar.jpeg)")]
    [InlineData("![Chart](http://example.com/data/chart.gif)")]
    [InlineData("![Screenshot](https://sub.domain.com/path/to/screenshot.webp)")]
    [InlineData("![Banner](https://example.org/images/banner.PNG)")]
    [InlineData("![Icon](http://static.example.com/icons/icon.ico)")]
    public void Map_WithCommentBodyThatContainsImageUrls_MaskImageUrls(string commentBody)
    {
        // arrange
        var ticket = ValidTicketDto;
        var comments = new[] { ValidCommentDto with { PlainBody = commentBody } };
        var employees = new[] { ValidUserDto };

        // act
        var result = ZendeskTicketMapper.Map(ticket, comments, employees);

        // assert
        result.Messages[0].Content.Should().Be("<IMAGE URL>");
    }

    [Theory]
    [InlineData(-1u)]
    [InlineData(null)]
    public void Map_WithCommentWithInvalidAuthorId_SetsUnknownAuthorCommentPlaceholder(long? authorId)
    {
        // arrange
        var ticket = ValidTicketDto;
        var comments = new[] { ValidCommentDto with { AuthorId = authorId } };
        var employees = new[] { ValidUserDto };

        // act
        var result = ZendeskTicketMapper.Map(ticket, comments, employees);

        // assert
        result.Messages[0].Author.Should().Be("<UNKNOWN AUTHOR>");
    }

    [Fact]
    public void Map_WithCommentWithAuthorThatDoesNotExist_SetsNonEmployeeCommentPlaceholder()
    {
        // arrange
        var ticket = ValidTicketDto;
        var comments = new[] { ValidCommentDto };
        UserDto[] employees = [];

        // act
        var result = ZendeskTicketMapper.Map(ticket, comments, employees);

        // assert
        result.Messages[0].Author.Should().Be("<NON-EMPLOYEE>");
    }

    [Fact]
    public void Map_WithCommentWithNullCreatedAt_ThrowsException()
    {
        // arrange
        var ticket = ValidTicketDto;
        var comments = new[] { ValidCommentDto with { CreatedAt = null } };
        var employees = new[] { ValidUserDto };

        // act
        var map = () => ZendeskTicketMapper.Map(ticket, comments, employees);

        // assert
        map.Should().Throw<Exception>().WithMessage("Could not parse Created At");
    }

    [Fact]
    public void Map_WithCommentWithInvalidCreatedAt_ThrowsException()
    {
        // arrange
        var ticket = ValidTicketDto;
        var comments = new[] { ValidCommentDto with { CreatedAt = "invalid-date" } };
        var employees = new[] { ValidUserDto };

        // act
        var map = () => ZendeskTicketMapper.Map(ticket, comments, employees);

        // assert
        map.Should().Throw<Exception>().WithMessage("Could not parse Created At");
    }

    private static TicketDto ValidTicketDto =>
        new(
            Url: null,
            Id: 1,
            ExternalId: null,
            Via: null,
            CreatedAt: "1970-01-01T00:00:00Z",
            UpdatedAt: null,
            GeneratedTimestamp: null,
            Type: null,
            Subject: null,
            RawSubject: null,
            Description: null,
            Priority: null,
            Status: null,
            Recipient: null,
            RequesterId: null,
            SubmitterId: null,
            AssigneeId: null,
            OrganizationId: null,
            GroupId: null,
            CollaboratorIds: null,
            FollowerIds: null,
            EmailCcIds: null,
            ForumTopicId: null,
            ProblemId: null,
            HasIncidents: null,
            IsPublic: null,
            DueAt: null,
            Tags: null,
            CustomFields: null,
            SatisfactionRating: null,
            SharingAgreementIds: null,
            CustomStatusId: null,
            EncodedId: null,
            Fields: null,
            FollowupIds: null,
            TicketFormId: null,
            BrandId: null,
            AllowChannelback: null,
            AllowAttachments: null,
            FromMessagingChannel: null,
            AssigneeEmail: null,
            AttributeValueIds: null,
            Collaborators: null,
            Comment: null,
            EmailCcs: null,
            Followers: null,
            MacroId: null,
            MacroIds: null,
            Metadata: null,
            Requester: null,
            SafeUpdate: null,
            UpdatedStamp: null,
            ViaFollowupSourceId: null,
            ViaId: null,
            VoiceComment: null
        );

    private static CommentDto ValidCommentDto =>
        new(
            Id: 10,
            Type: null,
            AuthorId: 100,
            Body: "Test Comment",
            HtmlBody: "Test Comment",
            PlainBody: "Test Comment",
            Public: null,
            Attachments: null,
            AuditId: null,
            Via: null,
            CreatedAt: "1970-01-01T00:00:00Z",
            Metadata: null,
            Uploads: null
        );

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