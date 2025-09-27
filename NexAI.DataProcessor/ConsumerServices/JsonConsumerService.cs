using Microsoft.Extensions.Logging;
using NexAI.DataProcessor.Zendesk;
using NexAI.RabbitMQ;
using NexAI.Zendesk;

namespace NexAI.DataProcessor.ConsumerServices;

public class JsonConsumerService(ILogger<JsonConsumerService> logger, RabbitMQClient rabbitMQClient, ZendeskTicketJsonExporter zendeskTicketJsonExporter)
    : RabbitMQConsumerService<ZendeskTicket>(new(logger, rabbitMQClient, zendeskTicketJsonExporter.CreateSchema, zendeskTicketJsonExporter.Export, "nexai.zendesk_tickets.json"));