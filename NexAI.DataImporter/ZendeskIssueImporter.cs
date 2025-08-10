using System.Text.Json;
using NexAI.Config;
using NexAI.Zendesk;

namespace NexAI.DataImporter;

internal class ZendeskIssueImporter(Options options)
{
    public async Task<ZendeskIssue[]> Import()
    {
        var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample-issues.json");
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

        return zendeskIssues;
    }
}