// ReSharper disable InconsistentNaming

using Neo4j.Driver;
using Neo4j.Driver.Mapping;
using NexAI.Config;

namespace NexAI.Neo4j;

public class Neo4jDbClient
{
    public IDriver Driver { get; }

    public Neo4jDbClient(Options options)
    {
        var neo4jOptions = options.Get<Neo4jOptions>();
        Driver = GraphDatabase.Driver(neo4jOptions.ConnectionString, AuthTokens.Basic(neo4jOptions.Username, neo4jOptions.Password));
    }

    public async Task ExecuteQuery(string query, IDictionary<string, object> parameters)
    {
        await using var session = Driver.AsyncSession(sessionConfigBuilder => sessionConfigBuilder.WithDatabase("neo4j"));
        await session.RunAsync(query, parameters);
    }
    
    public async Task<T[]> GetMany<T>(string query, IDictionary<string, object> parameters, IRecordMapper<T> mapper)
    {
        var result = await Driver.ExecutableQuery(query)
            .WithConfig(new QueryConfig(database: "neo4j"))
            .WithParameters(parameters)
            .WithMap(mapper.Map)
            .ExecuteAsync();
        return result.Result.ToArray();
    }
    
    public async Task<T?> GetOne<T>(string query, IDictionary<string, object> parameters, IRecordMapper<T> mapper)
    {
        var result = await Driver.ExecutableQuery(query)
            .WithConfig(new QueryConfig(database: "neo4j"))
            .WithParameters(parameters)
            .WithMap(mapper.Map)
            .ExecuteAsync();
        return result.Result.SingleOrDefault();
    }
}