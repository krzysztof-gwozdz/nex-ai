using NexAI.Config;
using NexAI.RabbitMQ;
using RabbitMQ.Client;

namespace NexAI.DataImporter;

public class RabbitMQ(Options options)
{
    public const string ExchangeName = "data_importer";
    
    public async Task CreateStructure()
    {
        var rabbitMQClient = new RabbitMQClient(options.Get<RabbitMQOptions>());
        await using var connection = await rabbitMQClient.ConnectionFactory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
    
        await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Fanout, durable: true, autoDelete: false);
        await channel.QueueDeclareAsync("mongodb", durable: true, exclusive: false, autoDelete: false);
        await channel.QueueBindAsync("mongodb", ExchangeName, "");
        await channel.QueueDeclareAsync("qdrant", durable: true, exclusive: false, autoDelete: false);
        await channel.QueueBindAsync("qdrant", ExchangeName, "");
        await channel.QueueDeclareAsync("json", durable: true, exclusive: false, autoDelete: false);
        await channel.QueueBindAsync("json", ExchangeName, "");
    }
}