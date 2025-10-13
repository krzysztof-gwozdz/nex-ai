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
            CREATE (user:User { 
                id: $id, 
                externalId: $externalId, 
                name: $name
            })";
        var parameters = new Dictionary<string, object>
        {
            { "id", (string)zendeskUser.Id },
            { "externalId", zendeskUser.ExternalId },
            { "name", zendeskUser.Name }
        };
        await neo4jDbClient.ExecuteQuery(query, parameters);
        AnsiConsole.MarkupLine($"[deepskyblue1]Successfully exported Zendesk user {zendeskUser.Id} into Neo4j.[/]");
    }
}