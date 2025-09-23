using NexAI.RabbitMQ;
using RabbitMQ.Client;

namespace NexAI.DataImporter;

public class RabbitMQStructure(RabbitMQClient rabbitMQClient)
{
    public const string ExchangeName = "nexai.zendesk_tickets";
    
    public async Task Create()
    {
        await using var connection = await rabbitMQClient.ConnectionFactory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
    
        await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Fanout, durable: true, autoDelete: false);
        await channel.QueueDeclareAsync("nexai.zendesk_tickets.mongodb", durable: true, exclusive: false, autoDelete: false);
        await channel.QueueBindAsync("nexai.zendesk_tickets.mongodb", ExchangeName, "");
        await channel.QueueDeclareAsync("nexai.zendesk_tickets.qdrant", durable: true, exclusive: false, autoDelete: false);
        await channel.QueueBindAsync("nexai.zendesk_tickets.qdrant", ExchangeName, "");
        await channel.QueueDeclareAsync("nexai.zendesk_tickets.json", durable: true, exclusive: false, autoDelete: false);
        await channel.QueueBindAsync("nexai.zendesk_tickets.json", ExchangeName, "");
    }
}