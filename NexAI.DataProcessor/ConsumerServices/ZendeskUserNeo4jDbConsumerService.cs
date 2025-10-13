// ReSharper disable InconsistentNaming

using Microsoft.Extensions.Logging;
using NexAI.DataProcessor.Zendesk;
using NexAI.RabbitMQ;
using NexAI.Zendesk;

namespace NexAI.DataProcessor.ConsumerServices;

public class ZendeskUserNeo4jDbConsumerService(ILogger<ZendeskUserNeo4jDbConsumerService> logger, RabbitMQClient rabbitMQClient, ZendeskUserNeo4jExporter zendeskUserNeo4jExporter)
    : RabbitMQConsumerService<ZendeskUser>(new(logger, rabbitMQClient, zendeskUserNeo4jExporter.CreateSchema, zendeskUserNeo4jExporter.Export, "nexai.zendesk_users.neo4j"));