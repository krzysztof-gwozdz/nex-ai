// ReSharper disable InconsistentNaming

using Microsoft.Extensions.Logging;
using NexAI.DataProcessor.Zendesk;
using NexAI.ServiceBus;
using NexAI.Zendesk.Messages;

namespace NexAI.DataProcessor.ConsumerServices;

public class ZendeskUserGroupsNeo4jDbConsumerService(ILogger<ZendeskUserGroupsNeo4jDbConsumerService> logger, RabbitMQClient rabbitMQClient, ZendeskUserGroupsNeo4jExporter zendeskUserGroupsNeo4jExporter)
    : RabbitMQConsumerService<ZendeskUserGroupsImportedEvent>(new(logger, rabbitMQClient, zendeskUserGroupsNeo4jExporter.CreateSchema, zendeskUserGroupsNeo4jExporter.Export, "nexai.zendesk_users_groups.neo4j"));