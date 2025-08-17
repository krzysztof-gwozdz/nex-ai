using NexAI.Config;

namespace NexAI.DataImporter.Zendesk;

public class ZendeskIssueUpdater(Options options)
{
    public async Task Update()
    {
        var importer = new ZendeskIssueImporter(options);
        var zendeskIssues = await importer.Import();
        var exporter = new ZendeskIssueExporter(options);
        // await exporter.Export(zendeskIssues); // DO NOT UNCOMMENT UNLESS YOU USE TEST DATA OR LOCAL LLM
    }
}