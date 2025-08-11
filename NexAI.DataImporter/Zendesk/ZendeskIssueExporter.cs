using NexAI.Config;
using NexAI.Zendesk;
using Spectre.Console;

namespace NexAI.DataImporter.Zendesk;

public class ZendeskIssueExporter(Options options)
{
    public async Task Export(ZendeskIssue[] zendeskIssues)
    {
        AnsiConsole.MarkupLine("[yellow]Start exporting Zendesk issues...[/]");
        await new ZendeskIssueQdrantExporter(options).Export(zendeskIssues);
        await new ZendeskIssueMongoDbExporter(options).Export(zendeskIssues);
        AnsiConsole.MarkupLine("[green]Zendesk issues exported successfully.[/]");
    }
}