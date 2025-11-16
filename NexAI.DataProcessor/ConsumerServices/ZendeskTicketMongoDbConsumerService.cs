using Microsoft.Extensions.Logging;
using NexAI.DataProcessor.Zendesk;
using NexAI.RabbitMQ;
using NexAI.Zendesk.Messages;

namespace NexAI.DataProcessor.ConsumerServices;

public class ZendeskTicketMongoDbConsumerService(ILogger<ZendeskTicketMongoDbConsumerService> logger, RabbitMQClient rabbitMQClient, ZendeskTicketMongoDbExporter zendeskTicketMongoDbExporter)
    : RabbitMQConsumerService<ZendeskTicketImportedEvent>(new(logger, rabbitMQClient, zendeskTicketMongoDbExporter.CreateSchema, zendeskTicketMongoDbExporter.Export, "nexai.zendesk_tickets.mongodb"));