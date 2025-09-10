using System.Text.Encodings.Web;
using System.Text.Json;
using NexAI.Config;
using NexAI.Zendesk;

namespace NexAI.DataProcessor.Zendesk;

public class ZendeskTicketJsonExporter(Options options)
{
    private const string FilePath = "zendesk_tickets.json";

    public Task CreateSchema()
    {
        if (!File.Exists(FilePath) || options.Get<DataProcessorOptions>().Recreate)
        {
            File.Create(FilePath);
        }
        return Task.CompletedTask;
    }

    public async Task Export(ZendeskTicket zendeskTicket)
    {
        if (!File.Exists(FilePath) || options.Get<DataProcessorOptions>().Recreate)
        {
            var json = JsonSerializer.Serialize(zendeskTicket, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });
            await File.AppendAllTextAsync(FilePath, json);
        }
    }
}