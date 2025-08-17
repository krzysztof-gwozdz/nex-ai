using NexAI.Config;

namespace NexAI.DataImporter.Zendesk;

public class ZendeskTicketUpdater(Options options)
{
    public async Task Update()
    {
        var importer = new ZendeskTicketImporter(options);
        var zendeskTickets = await importer.Import();
        var exporter = new ZendeskTicketExporter(options);
        // await exporter.Export(zendeskTickets); // DO NOT UNCOMMENT UNLESS YOU USE TEST DATA OR LOCAL LLM
    }
}