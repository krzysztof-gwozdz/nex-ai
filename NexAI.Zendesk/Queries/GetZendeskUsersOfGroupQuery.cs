// ReSharper disable InconsistentNaming

using NexAI.Neo4j;
using NexAI.Zendesk.Neo4j;

namespace NexAI.Zendesk.Queries;

public class GetZendeskUsersOfGroupQuery(Neo4jDbClient neo4jDbClient)
{
    public async Task<ZendeskUser[]> Handle(ZendeskGroupId zendeskGroupId, int limit)
    {
        const string query = @" 
            MATCH (group:Group {id: $groupId})<-[:MEMBER_OF]-(user:User) 
            RETURN user.id AS id, user.zendeskId AS zendeskId, user.name AS name, user.email AS email
            LIMIT $limit
        ";
        var parameters = new Dictionary<string, object>
        {
            { "groupId", (string)zendeskGroupId },
            { "limit", limit }
        };
        var mapper = new ZendeskUserNeo4jRecordMapper();
        return await neo4jDbClient.GetMany(query, parameters, mapper);
    }
}