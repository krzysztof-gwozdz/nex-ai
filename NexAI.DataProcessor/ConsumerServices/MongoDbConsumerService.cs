using Microsoft.Extensions.Logging;
using NexAI.DataProcessor.Zendesk;
using NexAI.RabbitMQ;
using NexAI.Zendesk;

namespace NexAI.DataProcessor.ConsumerServices;

public class MongoDbConsumerService(ILogger<MongoDbConsumerService> logger, RabbitMQClient rabbitMQClient, ZendeskTicketMongoDbExporter zendeskTicketMongoDbExporter)
    : RabbitMQConsumerService<ZendeskTicket>(new(logger, rabbitMQClient, zendeskTicketMongoDbExporter.CreateSchema, zendeskTicketMongoDbExporter.Export, "nexai.zendesk_tickets.mongodb"));