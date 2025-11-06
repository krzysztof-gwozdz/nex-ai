// ReSharper disable InconsistentNaming

using System.Text.Json;
using Neo4j.Driver;
using NexAI.Neo4j;

namespace NexAI.Zendesk.Queries;

public class GetInfoAboutZendeskHierarchy(Neo4jDbClient neo4jDbClient)
{
    public async Task<string> Handle(string cypherQuery)
    {
        await using var session = neo4jDbClient.Driver.AsyncSession(sessionConfigBuilder => sessionConfigBuilder.WithDatabase("neo4j"));
        try
        {
            var result = await session.ExecuteReadAsync(async queryRunner =>
            {
                var cursor = await queryRunner.RunAsync(cypherQuery);
                var records = await cursor.ToListAsync(record =>
                {
                    var dict = new Dictionary<string, object>();
                    foreach (var key in record.Keys)
                    {
                        dict[key] = ConvertValue(record[key]);
                    }
                    return dict;
                });
                return records;
            });
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize(result, options);
        }
        catch (Exception exception)
        {
            return exception.Message;
        }
    }

    private static object ConvertValue(object value) =>
        value switch
        {
            INode node => new
            {
                id = node.Id,
                labels = node.Labels,
                properties = node.Properties
            },
            IRelationship relationship => new
            {
                id = relationship.Id,
                type = relationship.Type,
                startNodeId = relationship.StartNodeId,
                endNodeId = relationship.EndNodeId,
                properties = relationship.Properties
            },
            IPath path => new
            {
                nodes = path.Nodes.Select(ConvertValue).ToList(),
                relationships = path.Relationships.Select(ConvertValue).ToList()
            },
            _ => value
        };
}