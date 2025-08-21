using System.Text.Encodings.Web;
using System.Text.Json;
using NexAI.Config;
using NexAI.Zendesk;

namespace NexAI.DataImporter.Zendesk;

public class ZendeskTicketJsonExporter(Options options)
{
    private const string FilePath = "zendesk_tickets.json";
    
    public async Task Export(ZendeskTicket[] zendeskTickets)
    {
        if (!File.Exists(FilePath) || options.Get<DataImporterOptions>().Recreate) 
        {
            var json = JsonSerializer.Serialize(zendeskTickets, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });
            await File.WriteAllTextAsync(FilePath, json);
        }
    }
}