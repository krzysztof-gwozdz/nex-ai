// ReSharper disable InconsistentNaming

using Neo4j.Driver;
using NexAI.Neo4j;
using NexAI.Zendesk.Neo4j;

namespace NexAI.Zendesk.Queries;

public class GetZendeskGroupByNameQuery(Neo4jDbClient neo4jDbClient)
{
    public async Task<ZendeskGroup?> Handle(string zendeskGroupName)
    {
        const string query = @" 
            MATCH (group:Group {name: $groupName})
            RETURN group.id AS id, group.zendeskId AS zendeskId, group.name AS name
            LIMIT 1
        ";
        var parameters = new Dictionary<string, object>
        {
            { "groupName", zendeskGroupName }
        };
        var mapper = new ZendeskGroupNeo4jRecordMapper();
        return await neo4jDbClient.GetOne(query, parameters, mapper);
    }
}