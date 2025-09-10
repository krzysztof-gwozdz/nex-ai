using System.Text;
using System.Text.Json;
using NexAI.Config;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NexAI.RabbitMQ;

public abstract class RabbitMQConsumer<TMessage>(Options options, string queueName)
{
    private readonly RabbitMQClient _client = new(options.Get<RabbitMQOptions>());

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await using var connection = await _client.ConnectionFactory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, deliverEventArgs) =>
        {
            var body = deliverEventArgs.Body.ToArray();
            var messageString = Encoding.UTF8.GetString(body);
            var message = JsonSerializer.Deserialize<TMessage>(messageString) ?? throw new($"Failed to deserialize message from '{queueName}' queue.");
            await HandleMessage(message);
        };
        await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer, cancellationToken: cancellationToken);
        var taskCompletionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await using var _ = cancellationToken.Register(() => taskCompletionSource.TrySetResult());
        await taskCompletionSource.Task;
    }

    protected abstract Task HandleMessage(TMessage message);
}