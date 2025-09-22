using Microsoft.Extensions.Logging;
using NexAI.Config;
using NexAI.DataProcessor.Zendesk;
using NexAI.RabbitMQ;
using NexAI.Zendesk;

namespace NexAI.DataProcessor.ConsumerServices;

public class JsonConsumerService(ILogger<JsonConsumerService> logger, RabbitMQClient rabbitMQClient, Options options)
    : RabbitMQConsumerService<ZendeskTicket>(new(logger, rabbitMQClient, zendeskTicket => new ZendeskTicketJsonExporter(options).Export(zendeskTicket), "json"));