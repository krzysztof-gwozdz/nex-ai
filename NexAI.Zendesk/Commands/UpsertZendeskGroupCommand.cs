// ReSharper disable InconsistentNaming

using NexAI.Neo4j;

namespace NexAI.Zendesk.Commands;

public class UpsertZendeskGroupCommand(Neo4jDbClient neo4jDbClient)
{
    public async Task Handle(ZendeskGroup zendeskGroup)
    {
        const string query = @"
            MERGE (group:Group { zendeskId: $zendeskId })
            ON CREATE SET group.id = $id
            SET group.name = $name";
        var parameters = new Dictionary<string, object>
        {
            { "id", (string)zendeskGroup.Id },
            { "zendeskId", zendeskGroup.ExternalId },
            { "name", zendeskGroup.Name }
        };
        await neo4jDbClient.ExecuteQuery(query, parameters);
    }
}