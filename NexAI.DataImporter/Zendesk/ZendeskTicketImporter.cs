using System.Text.Json;
using NexAI.Config;
using NexAI.RabbitMQ;
using NexAI.Zendesk;
using NexAI.Zendesk.Api;
using NexAI.Zendesk.Api.Dtos;
using Spectre.Console;

namespace NexAI.DataImporter.Zendesk;

internal class ZendeskTicketImporter(ZendeskApiClient zendeskApiClient, RabbitMQClient rabbitMQClient, Options options)
{
    private const string BackupGroupsFilePath = "zendesk_api_backup_groups.json";
    private const string BackupEmployeesFilePath = "zendesk_api_backup_employees.json";
    private const string BackupTicketsFilePath = "zendesk_api_backup_tickets_{0:yyyyMMdd}.json";
    private const string BackupCommentsFilePath = "zendesk_api_backup_{0}_comments.json";

    private readonly DateTime _ticketStartDateTime = new(2025, 1, 1);

    public async Task Import()
    {
        AnsiConsole.MarkupLine("[yellow]Importing sample Zendesk tickets from JSON...[/]");
        var groups = await GetGroupsFromApiOrBackup();
        var employees = await GetEmployeesFromApiOrBackup();
        var tickets = await GetTicketsFromApiOrBackup(_ticketStartDateTime);
        var zendeskTickets = new List<ZendeskTicket>();
        foreach (var ticket in tickets)
        {
            var comments = await GetCommentsFromApiOrBackup(ticket.Id!.Value);
            zendeskTickets.Add(ZendeskTicketMapper.Map(ticket, comments, employees));
        }
        AnsiConsole.MarkupLine($"[green]Successfully imported {zendeskTickets.Count} Zendesk tickets.[/]");
        AnsiConsole.MarkupLine("[darkgoldenrod]Sending Zendesk ticket to RabbitMQStructure...[/]");
        await rabbitMQClient.Send(RabbitMQStructure.ExchangeName, zendeskTickets);
    }

    private async Task<GroupDto[]> GetGroupsFromApiOrBackup() =>
        await GetFromApiOrBackup(
            BackupGroupsFilePath,
            zendeskApiClient.GetGroups,
            "Groups",
            group => $"Fetched {group.Length} groups from Zendesk.");

    private async Task<UserDto[]> GetEmployeesFromApiOrBackup() =>
        await GetFromApiOrBackup(
            BackupEmployeesFilePath,
            () => zendeskApiClient.GetEmployees(),
            "Employees",
            user => $"Fetched {user.Length} employees from Zendesk.");

    private async Task<TicketDto[]> GetTicketsFromApiOrBackup(DateTime startTime) =>
        await GetFromApiOrBackup(
            string.Format(BackupTicketsFilePath, startTime),
            () => zendeskApiClient.GetTickets(startTime),
            "Tickets",
            ticket => $"Fetched {ticket.Length} tickets from Zendesk.");

    private async Task<CommentDto[]> GetCommentsFromApiOrBackup(long ticketId) =>
        await GetFromApiOrBackup(
            string.Format(BackupCommentsFilePath, ticketId),
            () => zendeskApiClient.GetTicketComments(ticketId),
            $"Comments for ticket {ticketId}",
            comment => $"Fetched {comment.Length} comments from Zendesk for ticket {ticketId}.");

    private async Task<T[]> GetFromApiOrBackup<T>(string filename, Func<Task<T[]>> fetchFromApi, string entityDescription, Func<T[], string> fetchedMessage)
    {
        var filePath = GetBackupFilePath(filename);
        if (options.Get<DataImporterOptions>().UseBackup && await TryLoadFromBackup<T[]>(filePath) is { } backup)
        {
            AnsiConsole.MarkupLine($"[blue]Found backup for {entityDescription}. Loading {backup.Length} from backup file[/]");
            return backup;
        }
        AnsiConsole.MarkupLine("[green]Fetching from Zendesk.[/]");
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