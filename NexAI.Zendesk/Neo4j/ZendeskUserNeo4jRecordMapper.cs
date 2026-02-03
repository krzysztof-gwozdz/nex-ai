// ReSharper disable InconsistentNaming

using Neo4j.Driver;
using Neo4j.Driver.Mapping;

namespace NexAI.Zendesk.Neo4j;

public class ZendeskUserNeo4jRecordMapper : IRecordMapper<ZendeskUser>
{
    public ZendeskUser Map(IRecord record) =>
        new(new(Guid.Parse(record["id"].As<string>())),
            record["zendeskId"].As<string>(),
            record["name"].As<string>(),
            record["email"].As<string>());
}