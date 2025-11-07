// ReSharper disable InconsistentNaming

using NexAI.Neo4j;

namespace NexAI.Zendesk.Commands;

public class UpsertZendeskMembersOfRelationshipCommand(Neo4jDbClient neo4jDbClient)
{
    public async Task Handle(ZendeskUserGroups zendeskUserGroups)
    {
        const string query = @"
            MATCH (user:User {id: $userId})
            UNWIND $groupIds AS groupId
            MATCH (group:Group {id: groupId})
            MERGE (user)-[:MEMBER_OF]->(group)";

        var parameters = new Dictionary<string, object>
        {
            { "userId", (string)zendeskUserGroups.UserId },
            { "groupIds", zendeskUserGroups.Groups.Select(groupId => (string)groupId).ToList() }
        };

        await neo4jDbClient.ExecuteQuery(query, parameters);
    }
}