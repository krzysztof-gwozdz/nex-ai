using Microsoft.Extensions.Logging;
using NexAI.Config;
using NexAI.DataProcessor.Zendesk;
using NexAI.RabbitMQ;
using NexAI.Zendesk;

namespace NexAI.DataProcessor.ConsumerServices;

public class QdrantConsumerService(ILogger<QdrantConsumerService> logger, RabbitMQClient rabbitMQClient, Options options)
    : RabbitMQConsumerService<ZendeskTicket>(new(logger, rabbitMQClient, zendeskTicket => new ZendeskTicketQdrantExporter(options).Export(zendeskTicket), "nexai.zendesk_tickets.qdrant"));