using NexAI.Zendesk;
using NexAI.Zendesk.Commands;
using Spectre.Console;

namespace NexAI.DataProcessor.Zendesk;

public class ZendeskUserNeo4jExporter(UpsertZendeskUserCommand upsertZendeskUserCommand)
{
    public async Task CreateSchema(CancellationToken cancellationToken)
    {
        AnsiConsole.MarkupLine("[green]Current setup does not require schema creation for Zendesk tickets in Neo4j.[/]");
    }

    public async Task Export(ZendeskUser zendeskUser, CancellationToken cancellationToken)
    {
        await upsertZendeskUserCommand.Handle(zendeskUser);
        AnsiConsole.MarkupLine($"[deepskyblue1]Successfully exported Zendesk user {zendeskUser.Id} into Neo4j.[/]");
    }
}