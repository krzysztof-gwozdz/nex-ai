using RabbitMQ.Client;
using System.Text.Json;
using NexAI.Config;

namespace NexAI.RabbitMQ;

public class RabbitMQClient
{
    public ConnectionFactory ConnectionFactory { get; }

    public RabbitMQClient(Options options)
    {
        var rabbitMQOptions = options.Get<RabbitMQOptions>();
        ConnectionFactory = new() { HostName = rabbitMQOptions.Host, Port = rabbitMQOptions.Port, UserName = rabbitMQOptions.Username, Password = rabbitMQOptions.Password };
    }

    public async Task Send<TMessage>(string exchange, TMessage[] messages, CancellationToken cancellationToken)
    {
        await using var connection = await ConnectionFactory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        foreach (var message in messages)
        {
            var body = JsonSerializer.SerializeToUtf8Bytes(message);
            var properties = new BasicProperties
            {
                ContentType = "application/json",
                Persistent = true
            };
            await channel.BasicPublishAsync(exchange: exchange, routingKey: string.Empty, mandatory: false, basicProperties: properties, body: body, cancellationToken: cancellationToken);
        }
    }
}