// ReSharper disable InconsistentNaming

using Neo4j.Driver;
using NexAI.Config;

namespace NexAI.Neo4j;

public class Neo4jDbClient
{
    private readonly IDriver driver;

    public Neo4jDbClient(Options options)
    {
        var neo4jOptions = options.Get<Neo4jOptions>();
        driver = GraphDatabase.Driver(neo4jOptions.ConnectionString, AuthTokens.Basic(neo4jOptions.Username, neo4jOptions.Password));
    }

    public async Task<IResultCursor> RunQuery(string database, string query, IDictionary<string, object> parameters, Action<TransactionConfigBuilder>? action = null)
    {
        await using var session = driver.AsyncSession(sessionConfigBuilder => sessionConfigBuilder.WithDatabase(database));
        return await session.RunAsync(query, parameters, action);
    }
}