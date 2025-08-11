using System.Text.Json;
using NexAI.Config;
using NexAI.Zendesk;
using Spectre.Console;

namespace NexAI.DataImporter.Zendesk;

internal class ZendeskIssueImporter(Options options)
{
    public async Task<ZendeskIssue[]> Import()
    {
        AnsiConsole.MarkupLine("[yellow]Importing sample Zendesk issues from JSON...[/]");
        var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Zendesk", "sample-issues.json");
        if (!File.Exists(jsonPath))
        {
            throw new($"Sample issues file not found at: {jsonPath}");
        }

        var jsonContent = await File.ReadAllTextAsync(jsonPath);
        var zendeskIssues = JsonSerializer.Deserialize<ZendeskIssue[]>(jsonContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        if (zendeskIssues == null)
        {
            throw new("Failed to deserialize sample issues from JSON");
        }

        AnsiConsole.MarkupLine($"[green]Successfully imported {zendeskIssues.Length} Zendesk issues.[/]");
        zendeskIssues = zendeskIssues
            .Select(issue => issue.Id == Guid.Empty ? issue with { Id = Guid.NewGuid() } : issue)
            .ToArray();
        return zendeskIssues;
    }
}