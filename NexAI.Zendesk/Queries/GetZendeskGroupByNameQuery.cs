// ReSharper disable InconsistentNaming

using Neo4j.Driver;
using NexAI.Neo4j;

namespace NexAI.Zendesk.Queries;

public class GetZendeskGroupByNameQuery(Neo4jDbClient neo4jDbClient)
{
    public async Task<ZendeskGroup?> Handle(string zendeskGroupName)
    {
        const string query = @" 
            MATCH (group:Group {name: $groupName})
            RETURN group.id AS id, group.externalId AS externalId, group.name AS name
            LIMIT 1
        ";
            var parameters = new Dictionary<string, object>
            {
                { "groupName", zendeskGroupName }
            };
            await using var session = neo4jDbClient.Driver.AsyncSession(sessionConfigBuilder => sessionConfigBuilder.WithDatabase("neo4j"));
            var group = await session.ExecuteReadAsync(async queryRunner =>
            {
                var cursor = await queryRunner.RunAsync(query, parameters);
                var record = await cursor.SingleAsync();
                if (record is null)
                {
                    return null;
                }
                var group = new ZendeskGroup(
                    new(record["id"].As<Guid>()),
                    record["externalId"].As<string>(),
                    record["name"].As<string>());
                return group;
            });
            return group;
        }
}