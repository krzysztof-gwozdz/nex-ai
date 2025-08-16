using NexAI.Config;

namespace NexAI.DataImporter.Zendesk;

public class ZendeskIssueUpdater(Options options)
{
    public async Task Update()
    {
        var importer = new ZendeskIssueSampleDataImporter(options);
        var zendeskIssues = await importer.Import();
        var exporter = new ZendeskIssueExporter(options);
        await exporter.Export(zendeskIssues);
    }
}