// ReSharper disable InconsistentNaming

using NexAI.Neo4j;
using NexAI.Zendesk;
using Spectre.Console;

namespace NexAI.DataProcessor.Zendesk;

public class ZendeskUserNeo4jExporter(Neo4jDbClient neo4jDbClient)
{
    public async Task CreateSchema(CancellationToken cancellationToken)
    {
        AnsiConsole.MarkupLine("[green]Current setup does not require schema creation for Zendesk tickets in Neo4j.[/]");
    }

    public async Task Export(ZendeskUser zendeskUser, CancellationToken cancellationToken)
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
        AnsiConsole.MarkupLine($"[deepskyblue1]Successfully exported Zendesk user {zendeskUser.Id} into Neo4j.[/]");
    }
}