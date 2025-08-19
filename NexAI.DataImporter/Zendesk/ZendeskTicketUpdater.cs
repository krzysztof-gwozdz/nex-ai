using NexAI.Config;

namespace NexAI.DataImporter.Zendesk;

public class ZendeskTicketUpdater(Options options)
{
    public async Task Update()
    {
        var dataImporterOptions = options.Get<DataImporterOptions>();
        var importer = new ZendeskTicketImporter(options);
        var zendeskTickets = await importer.Import();
        var exporter = new ZendeskTicketExporter(options);
        await exporter.Export(zendeskTickets);
    }
}