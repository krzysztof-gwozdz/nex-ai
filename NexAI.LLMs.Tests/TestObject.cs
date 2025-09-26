using System.Text.Json.Serialization;

namespace NexAI.LLMs.Tests;

public record TestObject(
    [property: JsonPropertyName("FirstName")]
    string FirstName,
    [property: JsonPropertyName("LastName")]
    string LastName,
    [property: JsonPropertyName("Job")]
    string? Job
);