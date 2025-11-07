// ReSharper disable InconsistentNaming

using NexAI.Neo4j;
using NexAI.Zendesk;
using Spectre.Console;

namespace NexAI.DataProcessor.Zendesk;

public class ZendeskGroupNeo4jExporter(Neo4jDbClient neo4jDbClient)
{
    public async Task CreateSchema(CancellationToken cancellationToken)
    {
        AnsiConsole.MarkupLine("[green]Current setup does not require schema creation for Zendesk tickets in Neo4j.[/]");
    }

    public async Task Export(ZendeskGroup zendeskGroup, CancellationToken cancellationToken)
    {
        const string query = @"
            CREATE (group:Group { 
                id: $id, 
                zendeskId: $zendeskId, 
                name: $name
            })";
        var parameters = new Dictionary<string, object>
        {
            { "id", (string)zendeskGroup.Id },
            { "zendeskId", zendeskGroup.ExternalId },
            { "name", zendeskGroup.Name }
        };
        await neo4jDbClient.ExecuteQuery(query, parameters);
        AnsiConsole.MarkupLine($"[deepskyblue1]Successfully exported Zendesk group {zendeskGroup.ExternalId} into Neo4j.[/]");
    }
}