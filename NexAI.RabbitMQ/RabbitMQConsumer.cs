using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NexAI.RabbitMQ;

public class RabbitMQConsumer<TMessage>(ILogger logger, RabbitMQClient rabbitMQClient, Func<TMessage, Task> handleMessage, string queueName) : IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;

    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.DisposeAsync();
        }
        if (_channel != null)
        {
            await _channel.DisposeAsync();
        }
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Connecting to '{queueName}' queue.", queueName);
        _connection = await rabbitMQClient.ConnectionFactory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await _channel.BasicQosAsync(0, 10, false, cancellationToken);
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, deliverEventArgs) =>
        {
            try
            {
                var body = deliverEventArgs.Body.ToArray();
                var messageString = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<TMessage>(messageString) ?? throw new($"Failed to deserialize message from '{queueName}' queue.");
                await handleMessage(message);
                await _channel.BasicAckAsync(deliverEventArgs.DeliveryTag, multiple: false, cancellationToken: cancellationToken);
            }
            catch(Exception exception)
            {
                logger.LogError(exception, "Failed to handle message from '{queueName}' queue.", queueName);
                await _channel.BasicRejectAsync(deliverEventArgs.DeliveryTag, requeue: false, cancellationToken: cancellationToken);
            }
        };
        await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);
        var taskCompletionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await using var _ = cancellationToken.Register(() => taskCompletionSource.TrySetResult());
        await taskCompletionSource.Task;
    }
}