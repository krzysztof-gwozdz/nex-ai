// ReSharper disable InconsistentNaming

using Microsoft.Extensions.Logging;
using NexAI.DataProcessor.Git;
using NexAI.Git.Messages;
using NexAI.RabbitMQ;

namespace NexAI.DataProcessor.ConsumerServices;

public class GitCommitNeo4jDbConsumerService(ILogger<GitCommitNeo4jDbConsumerService> logger, RabbitMQClient rabbitMQClient, GitCommitNeo4jExporter gitCommitNeo4jExporter)
    : RabbitMQConsumerService<GitCommitImportedEvent>(new(logger, rabbitMQClient, gitCommitNeo4jExporter.CreateSchema, gitCommitNeo4jExporter.Export, "nexai.git_commits.neo4j"));
