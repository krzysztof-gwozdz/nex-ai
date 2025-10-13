// ReSharper disable InconsistentNaming

using NexAI.Neo4j;
using NexAI.Zendesk;
using Spectre.Console;

namespace NexAI.DataProcessor.Zendesk;

public class ZendeskUserGroupsNeo4jExporter(Neo4jDbClient neo4jDbClient)
{
    public async Task CreateSchema(CancellationToken cancellationToken)
    {
        AnsiConsole.MarkupLine("[green]Current setup does not require schema creation for Zendesk tickets in Neo4j.[/]");
    }

    public async Task Export(ZendeskUserGroups zendeskUserGroups, CancellationToken cancellationToken)
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
        AnsiConsole.MarkupLine($"[deepskyblue1]Successfully exported Zendesk user groups for user {zendeskUserGroups.UserId} into Neo4j.[/]");
    }
}