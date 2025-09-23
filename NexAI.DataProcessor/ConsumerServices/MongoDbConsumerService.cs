using Microsoft.Extensions.Logging;
using NexAI.Config;
using NexAI.DataProcessor.Zendesk;
using NexAI.RabbitMQ;
using NexAI.Zendesk;

namespace NexAI.DataProcessor.ConsumerServices;

public class MongoDbConsumerService(ILogger<MongoDbConsumerService> logger, RabbitMQClient rabbitMQClient, Options options)
    : RabbitMQConsumerService<ZendeskTicket>(new(logger, rabbitMQClient, zendeskTicket => new ZendeskTicketMongoDbExporter(options).Export(zendeskTicket), "nexai.zendesk_tickets.mongodb"));