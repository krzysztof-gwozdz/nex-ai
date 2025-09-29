using NexAI.RabbitMQ;
using RabbitMQ.Client;

namespace NexAI.DataImporter;

public class RabbitMQStructure(RabbitMQClient rabbitMQClient)
{
    public const string ExchangeName = "nexai.zendesk_tickets";
    
    public async Task Create(CancellationToken cancellationToken)
    {
        await using var connection = await rabbitMQClient.ConnectionFactory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
    
        await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Fanout, durable: true, autoDelete: false, cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync("nexai.zendesk_tickets.mongodb", durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
        await channel.QueueBindAsync("nexai.zendesk_tickets.mongodb", ExchangeName, "", cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync("nexai.zendesk_tickets.qdrant", durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
        await channel.QueueBindAsync("nexai.zendesk_tickets.qdrant", ExchangeName, "", cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync("nexai.zendesk_tickets.json", durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
        await channel.QueueBindAsync("nexai.zendesk_tickets.json", ExchangeName, "", cancellationToken: cancellationToken);
    }
}