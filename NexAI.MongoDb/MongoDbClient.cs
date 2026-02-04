using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using NexAI.Config;

namespace NexAI.MongoDb;

public class MongoDbClient
{
    public MongoDbClient(Options options)
    {
        var mongoDbOptions = options.Get<MongoDbOptions>();
        var clientSettings = MongoClientSettings.FromUrl(new(mongoDbOptions.ConnectionString));
        MongoClient client = new(clientSettings);
        Database = client.GetDatabase(mongoDbOptions.Database);
        RegisterGuidSerializer();
    }
    
    public IMongoDatabase Database { get; }

    private static void RegisterGuidSerializer()
    {
        try
        {
            BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        }
        catch
        {
            // ignored
        }
    }
}