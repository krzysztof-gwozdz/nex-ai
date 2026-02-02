// ReSharper disable InconsistentNaming

using Neo4j.Driver;
using NexAI.Neo4j;

namespace NexAI.Tests.Neo4j;

public static class Neo4jDbClientExtensions
{
    public static async Task CleanDatabase(this Neo4jDbClient neo4jDbClient)
    {
        await using var session = neo4jDbClient.Driver.AsyncSession(sessionConfigBuilder => sessionConfigBuilder.WithDatabase("neo4j"));
        await session.RunAsync("MATCH (n) DETACH DELETE n");
    }

    public static async Task<IRecord?> GetNode(this Neo4jDbClient neo4jDbClient, string label, string property, object value)
    {
        await using var session = neo4jDbClient.Driver.AsyncSession(sessionConfigBuilder => sessionConfigBuilder.WithDatabase("neo4j"));
        var result = await session.RunAsync(
            $"MATCH (n:{label} {{{property}: $value}}) RETURN n",
            new Dictionary<string, object> { { "value", value } });
        await result.FetchAsync();
        return result.Current;
    }

    public static async Task<IRecord> GetRelationship(this Neo4jDbClient neo4jDbClient, string fromLabel, string fromProperty, object fromValue, string relationshipType, string toLabel, string toProperty, object toValue)
    {
        await using var session = neo4jDbClient.Driver.AsyncSession(sessionConfigBuilder => sessionConfigBuilder.WithDatabase("neo4j"));
        var result = await session.RunAsync(
            $"MATCH (a:{fromLabel} {{{fromProperty}: $fromValue}})-[r:{relationshipType}]->(b:{toLabel} {{{toProperty}: $toValue}}) RETURN r",
            new Dictionary<string, object>
            {
                { "fromValue", fromValue },
                { "toValue", toValue }
            });
        await result.FetchAsync();
        return result.Current;
    }
}