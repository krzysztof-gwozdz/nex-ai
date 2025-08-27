using System.Text.Json;
using NexAI.Zendesk;
using Spectre.Console;

namespace NexAI.DataImporter.Zendesk;

internal class ZendeskTicketSampleDataImporter
{
    public async Task<ZendeskTicket[]> Import()
    {
        AnsiConsole.MarkupLine("[yellow]Importing sample Zendesk tickets from JSON...[/]");
        var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Zendesk", "sample-tickets.json");
        if (!File.Exists(jsonPath))
        {
            throw new($"Sample tickets file not found at: {jsonPath}");
        }

        var jsonContent = await File.ReadAllTextAsync(jsonPath);
        var zendeskTickets = JsonSerializer.Deserialize<ZendeskTicket[]>(jsonContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        if (zendeskTickets == null)
        {
            throw new("Failed to deserialize sample tickets from JSON");
        }

        AnsiConsole.MarkupLine($"[green]Successfully imported {zendeskTickets.Length} Zendesk tickets.[/]");
        zendeskTickets = zendeskTickets
            .Select(ticket => ticket.Id == Guid.Empty ? ticket with { Id = ZendeskTicketId.New() } : ticket)
            .ToArray();
        return zendeskTickets;
    }
}