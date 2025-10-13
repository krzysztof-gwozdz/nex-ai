// ReSharper disable InconsistentNaming

using Microsoft.Extensions.Logging;
using NexAI.DataProcessor.Zendesk;
using NexAI.RabbitMQ;
using NexAI.Zendesk;

namespace NexAI.DataProcessor.ConsumerServices;

public class ZendeskGroupNeo4jDbConsumerService(ILogger<ZendeskGroupNeo4jDbConsumerService> logger, RabbitMQClient rabbitMQClient, ZendeskGroupNeo4jExporter zendeskGroupNeo4jExporter)
    : RabbitMQConsumerService<ZendeskGroup>(new(logger, rabbitMQClient, zendeskGroupNeo4jExporter.CreateSchema, zendeskGroupNeo4jExporter.Export, "nexai.zendesk_groups.neo4j"));