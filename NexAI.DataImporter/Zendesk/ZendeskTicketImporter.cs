using System.Text.Json;
using NexAI.Config;
using NexAI.Zendesk;
using NexAI.Zendesk.Api;
using NexAI.Zendesk.Api.Dtos;
using Spectre.Console;

namespace NexAI.DataImporter.Zendesk;

internal class ZendeskTicketImporter(Options options)
{
    private const string BackupGroupsFilePath = "zendesk_api_backup_groups.json";
    private const string BackupEmployeesFilePath = "zendesk_api_backup_employees.json";
    private const string BackupTicketsFilePath = "zendesk_api_backup_tickets.json";
    private const string BackupCommentsFilePath = "zendesk_api_backup_{0}_comments.json";
    
    private readonly DateTime _ticketStartDateTime = new(2025, 1, 1);

    public async Task<ZendeskTicket[]> Import()
    {
        AnsiConsole.MarkupLine("[yellow]Importing sample Zendesk tickets from JSON...[/]");
        var zendeskApiClient = new ZendeskApiClient(options);
        var groups = await GetGroupsFromApiOrBackup(zendeskApiClient);
        var employees = await GetEmployeesFromApiOrBackup(zendeskApiClient);
        var tickets = await GetTicketsFromApiOrBackup(zendeskApiClient);
        var zendeskTickets = new List<ZendeskTicket>();
        foreach (var ticket in tickets)
        {
            var comments = await GetCommentsFromApiOrBackup(zendeskApiClient, ticket.Id!.Value);
            zendeskTickets.Add(ZendeskTicketMapper.Map(ticket, comments, employees));
        }
        AnsiConsole.MarkupLine($"[green]Successfully imported {zendeskTickets.Count} Zendesk tickets.[/]");
        return zendeskTickets.ToArray();
    }

    private async Task<GroupDto[]> GetGroupsFromApiOrBackup(ZendeskApiClient zendeskApiClient) =>
        await GetFromApiOrBackup(
            BackupGroupsFilePath,
            zendeskApiClient.GetGroups,
            "Groups",
            group => $"Fetched {group.Length} groups from Zendesk.");

    private async Task<UserDto[]> GetEmployeesFromApiOrBackup(ZendeskApiClient zendeskApiClient) =>
        await GetFromApiOrBackup(
            BackupEmployeesFilePath,
            () => zendeskApiClient.GetEmployees(),
            "Employees",
            user => $"Fetched {user.Length} employees from Zendesk.");

    private async Task<TicketDto[]> GetTicketsFromApiOrBackup(ZendeskApiClient zendeskApiClient) =>
        await GetFromApiOrBackup(
            BackupTicketsFilePath,
            () => zendeskApiClient.GetTickets(_ticketStartDateTime),
            "Tickets",
            ticket => $"Fetched {ticket.Length} tickets from Zendesk.");

    private async Task<CommentDto[]> GetCommentsFromApiOrBackup(ZendeskApiClient zendeskApiClient, long ticketId) =>
        await GetFromApiOrBackup(
            string.Format(BackupCommentsFilePath, ticketId),
            () => zendeskApiClient.GetTicketComments(ticketId),
            $"Comments for ticket {ticketId}",
            comment => $"Fetched {comment.Length} comments from Zendesk for ticket {ticketId}.");

    private async Task<T> GetFromApiOrBackup<T>(string filename, Func<Task<T>> fetchFromApi, string entityDescription, Func<T, string> fetchedMessage)
    {
        var filePath = GetBackupFilePath(filename);
        if (options.Get<DataImporterOptions>().UseBackup && await TryLoadFromBackup<T>(filePath) is {} backup)
        {
            AnsiConsole.MarkupLine($"[green]{entityDescription} backup found, using it.[/]");
            return backup;
        }
        AnsiConsole.MarkupLine($"[yellow]No {entityDescription} backup found, fetching from Zendesk.[/]");
        var data = await fetchFromApi();
        AnsiConsole.MarkupLine($"[green]{fetchedMessage(data)}[/]");
        await BackupToFile(filePath, data);
        AnsiConsole.MarkupLine($"[green]{entityDescription} backup created.[/]");
        return data;
    }
    
    private static string GetBackupFilePath(string fileName)
    {
        var tempDirectory = Path.Combine(Path.GetTempPath(), "nexai", "zendesk");
        if (!Directory.Exists(tempDirectory))
        {
            Directory.CreateDirectory(tempDirectory);
        }
        return Path.Combine(tempDirectory, fileName);
    }

    private static async Task<T?> TryLoadFromBackup<T>(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return default;
        }
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<T>(json);
    }

    private static async Task BackupToFile<T>(string filePath, T data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
    }
}