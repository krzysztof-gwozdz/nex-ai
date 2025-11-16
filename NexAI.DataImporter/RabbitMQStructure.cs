using NexAI.RabbitMQ;
using RabbitMQ.Client;

namespace NexAI.DataImporter;

public class RabbitMQStructure(RabbitMQClient rabbitMQClient)
{
    private const string Prefix = "nexai.";
    public const string ZendeskTicketExchangeName = $"{Prefix}zendesk_tickets";
    public const string ZendeskGroupExchangeName = $"{Prefix}zendesk_groups";
    public const string ZendeskUserExchangeName = $"{Prefix}zendesk_users";
    public const string ZendeskUsersAndGroupsExchangeName = $"{Prefix}zendesk_users_groups";
    public const string GitCommitExchangeName = $"{Prefix}git_commits";

    public async Task Create(CancellationToken cancellationToken)
    {
        await using var connection = await rabbitMQClient.ConnectionFactory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await CreateZendeskTicketsExchangeWithQueues(channel, cancellationToken);
        await CreateZendeskUserAndGroupsExchangeWithQueues(channel, cancellationToken);
        await CreateGitExchangeWithQueues(channel, cancellationToken);
    }

    private static async Task CreateZendeskTicketsExchangeWithQueues(IChannel channel, CancellationToken cancellationToken)
    {
        await channel.ExchangeDeclareAsync(exchange: ZendeskTicketExchangeName, type: ExchangeType.Fanout, durable: true, autoDelete: false, cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync($"{ZendeskTicketExchangeName}.mongodb", durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
        await channel.QueueBindAsync($"{ZendeskTicketExchangeName}.mongodb", ZendeskTicketExchangeName, "", cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync($"{ZendeskTicketExchangeName}.qdrant", durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
        await channel.QueueBindAsync($"{ZendeskTicketExchangeName}.qdrant", ZendeskTicketExchangeName, "", cancellationToken: cancellationToken);
    }

    private static async Task CreateZendeskUserAndGroupsExchangeWithQueues(IChannel channel, CancellationToken cancellationToken)
    {
        await channel.ExchangeDeclareAsync(exchange: ZendeskGroupExchangeName, type: ExchangeType.Fanout, durable: true, autoDelete: false, cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync($"{ZendeskGroupExchangeName}.neo4j", durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
        await channel.QueueBindAsync($"{ZendeskGroupExchangeName}.neo4j", ZendeskGroupExchangeName, "", cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(exchange: ZendeskUserExchangeName, type: ExchangeType.Fanout, durable: true, autoDelete: false, cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync($"{ZendeskUserExchangeName}.neo4j", durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
        await channel.QueueBindAsync($"{ZendeskUserExchangeName}.neo4j", ZendeskUserExchangeName, "", cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(exchange: ZendeskUsersAndGroupsExchangeName, type: ExchangeType.Fanout, durable: true, autoDelete: false, cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync($"{ZendeskUsersAndGroupsExchangeName}.neo4j", durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
        await channel.QueueBindAsync($"{ZendeskUsersAndGroupsExchangeName}.neo4j", ZendeskUsersAndGroupsExchangeName, "", cancellationToken: cancellationToken);
    }

    private static async Task CreateGitExchangeWithQueues(IChannel channel, CancellationToken cancellationToken)
    {
        await channel.ExchangeDeclareAsync(exchange: GitCommitExchangeName, type: ExchangeType.Fanout, durable: true, autoDelete: false, cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync($"{GitCommitExchangeName}.neo4j", durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
        await channel.QueueBindAsync($"{GitCommitExchangeName}.neo4j", GitCommitExchangeName, "", cancellationToken: cancellationToken);
    }
}