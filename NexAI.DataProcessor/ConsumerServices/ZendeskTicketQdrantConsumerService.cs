using Microsoft.Extensions.Logging;
using NexAI.DataProcessor.Zendesk;
using NexAI.RabbitMQ;
using NexAI.Zendesk.Messages;

namespace NexAI.DataProcessor.ConsumerServices;

public class ZendeskTicketQdrantConsumerService(ILogger<ZendeskTicketQdrantConsumerService> logger, RabbitMQClient rabbitMQClient, ZendeskTicketQdrantExporter zendeskTicketQdrantExporter)
    : RabbitMQConsumerService<ZendeskTicketImportedEvent>(new(logger, rabbitMQClient, zendeskTicketQdrantExporter.CreateSchema, zendeskTicketQdrantExporter.Export,   "nexai.zendesk_tickets.qdrant"));