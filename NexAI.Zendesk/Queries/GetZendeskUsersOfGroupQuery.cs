// ReSharper disable InconsistentNaming

using Neo4j.Driver;
using NexAI.Neo4j;

namespace NexAI.Zendesk.Queries;

public class GetZendeskUsersOfGroupQuery(Neo4jDbClient neo4jDbClient)
{
    public async Task<ZendeskUser[]> Handle(ZendeskGroupId zendeskGroupId, int limit)
    {
        const string query = @" 
            MATCH (group:Group {id: $groupId})<-[:MEMBER_OF]-(user:User) 
            RETURN user.id AS id, user.externalId AS externalId, user.name AS name, user.email AS email
            LIMIT $limit
        ";
        var parameters = new Dictionary<string, object>
        {
            { "$groupId", (string)zendeskGroupId },
            { "$limit", limit }
        };
        await using var session = neo4jDbClient.Driver.AsyncSession(sessionConfigBuilder => sessionConfigBuilder.WithDatabase("neo4j"));
        var users = await session.ExecuteReadAsync(async queryRunner =>
        {
            var cursor = await queryRunner.RunAsync(query, parameters);
            var users = await cursor.ToListAsync<ZendeskUser>(record =>
                new(
                    new(record["id"].As<Guid>()),
                    record["externalId"].As<string>(),
                    record["name"].As<string>(),
                    record["email"].As<string>())
            );
            return users;
        });
        return users?.ToArray() ?? [];
    }
}