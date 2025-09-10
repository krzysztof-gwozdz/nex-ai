using RabbitMQ.Client;
using System.Text.Json;

namespace NexAI.RabbitMQ;

public class RabbitMQClient(RabbitMQOptions options)
{
    public ConnectionFactory ConnectionFactory { get; } = new() { HostName = options.Host, Port = options.Port, UserName = options.Username, Password = options.Password };

    public async Task Send<TMessage>(string exchange, TMessage message)
    {
        await using var connection = await ConnectionFactory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        var body = JsonSerializer.SerializeToUtf8Bytes(message);
        var properties = new BasicProperties
        {
            ContentType = "application/json",
            Persistent = true
        };
        await channel.BasicPublishAsync(exchange: exchange, routingKey: string.Empty, mandatory: false, basicProperties: properties, body: body);
    }
}