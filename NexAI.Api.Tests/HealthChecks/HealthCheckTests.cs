using System.Net;
using System.Text.Json.Serialization;

namespace NexAI.Api.Tests.HealthChecks;

public sealed class HealthCheckTests(NexAIApiApplicationFactory factory) : TestsBase(factory)
{
    [Fact]
    public async Task Get_Health_ReturnsStatusWithEntries()
    {
        // arrange
        const string url = "/health";

        // act
        var response = await Get(url);

        // assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.ServiceUnavailable);
        var report = await GetResponse<HealthReport>(response);
        report.Should().NotBeNull();
        report.Status.Should().BeOneOf("Healthy", "Unhealthy", "Degraded");
        report.Entries.Should().NotBeNull();

        report.Entries.Should().ContainKey("mongodb", because: "MongoDB health check should be registered");
        report.Entries["mongodb"].Status.Should().BeOneOf("Healthy", "Unhealthy", "Degraded");
        report.Entries.Should().ContainKey("neo4j", because: "Neo4j health check should be registered");
        report.Entries["neo4j"].Status.Should().BeOneOf("Healthy", "Unhealthy", "Degraded");
        report.Entries.Should().ContainKey("qdrant", because: "Qdrant health check should be registered");
        report.Entries["qdrant"].Status.Should().BeOneOf("Healthy", "Unhealthy", "Degraded");
    }

    [Fact]
    public async Task Get_HealthLive_ReturnsStatus()
    {
        // arrange
        const string url = "/health/live";

        // act
        var response = await Get(url);

        // assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.ServiceUnavailable);
        var report = await GetResponse<HealthReport>(response);
        report.Should().NotBeNull();
        report.Status.Should().BeOneOf("Healthy", "Unhealthy", "Degraded");
        report.Entries.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_HealthReady_ReturnsStatus()
    {
        // arrange
        const string url = "/health/ready";

        // act
        var response = await Get(url);

        // assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.ServiceUnavailable);
        var report = await GetResponse<HealthReport>(response);
        report.Should().NotBeNull();
        report.Status.Should().BeOneOf("Healthy", "Unhealthy", "Degraded");
        report.Entries.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_HealthChecksUi_ReturnsOkAndHtml()
    {
        // arrange
        const string url = "/healthchecks-ui";

        // act
        var response = await Get(url);

        // assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().NotBeNullOrWhiteSpace();
        body.Should().Contain("healthchecks", because: "UI page should reference healthchecks");
    }

    private sealed record HealthReport
    (
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("entries")] Dictionary<string, HealthCheckEntry> Entries
    );

    private sealed record HealthCheckEntry
    (
        [property: JsonPropertyName("status")] string Status
    );
}
