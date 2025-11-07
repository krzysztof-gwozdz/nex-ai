// ReSharper disable InconsistentNaming

using NexAI.Neo4j;

namespace NexAI.Zendesk.Commands;

public class UpsertZendeskUserCommand(Neo4jDbClient neo4jDbClient)
{
    public async Task Handle(ZendeskUser zendeskUser)
    {
        const string query = @"
            MERGE (user:User { zendeskId: $zendeskId })
            ON CREATE SET user.id = $id, user.name = $name, user.email = $email
            ON MATCH SET user.name = $name, user.email = $email";
        var parameters = new Dictionary<string, object>
        {
            { "id", (string)zendeskUser.Id },
            { "zendeskId", zendeskUser.ExternalId },
            { "name", zendeskUser.Name },
            { "email", zendeskUser.Email }
        };
        await neo4jDbClient.ExecuteQuery(query, parameters);
    }
}