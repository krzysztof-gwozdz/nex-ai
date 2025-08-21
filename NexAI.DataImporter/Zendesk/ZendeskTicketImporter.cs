using System.Text.Json;
using NexAI.Config;
using NexAI.Zendesk;
using NexAI.Zendesk.Api;
using Spectre.Console;

namespace NexAI.DataImporter.Zendesk;


internal class ZendeskTicketImporter(Options options)
{
    private const string BackupGroupsFilePath = "zendesk_api_backup_groups.json";
    private const string BackupEmployeesFilePath = "zendesk_api_backup_employees.json";
    private const string BackupTicketsFilePath = "zendesk_api_backup_tickets.json";
    private const string BackupCommentsFilePath = "zendesk_api_backup_{0}_comments.json";
    
    public async Task<ZendeskTicket[]> Import()
    {
        AnsiConsole.MarkupLine("[yellow]Importing sample Zendesk tickets from JSON...[/]");
        var zendeskApiClient = new ZendeskApiClient(options);
        var groups = await GetGroups(zendeskApiClient);
        var employees = await GetEmployees(zendeskApiClient);
        var tickets = await GetTickets(zendeskApiClient);
        var zendeskTickets = new List<ZendeskTicket>();
        foreach (var ticket in tickets)
        {
            var comments = await GetComments(zendeskApiClient, ticket.Id!.Value);
            zendeskTickets.Add(ZendeskTicketMapper.Map(ticket, comments, employees));
        }
        AnsiConsole.MarkupLine($"[green]Successfully imported {zendeskTickets.Count} Zendesk tickets.[/]");
        return zendeskTickets.ToArray();
    }

    private static async Task<ListGroupsDto.GroupDto[]> GetGroups(ZendeskApiClient zendeskApiClient)
    {
        var groups = await TryGetGroupsFromBackup();
        if (groups is null)
        {
            AnsiConsole.MarkupLine("[yellow]No groups backup found, fetching from Zendesk.[/]");
            groups = await zendeskApiClient.GetGroups();
            AnsiConsole.MarkupLine($"[green]Fetched {groups.Length} groups from Zendesk.[/]");
            await BackupGroups(groups);
            AnsiConsole.MarkupLine("[green]Groups backup created.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[green]Groups backup found, using it.[/]");
        }
        return groups;
    }

    private static async Task<ListGroupsDto.GroupDto[]?> TryGetGroupsFromBackup()
    {
        var filePath = GetBackupFilePath(BackupGroupsFilePath);
        if (!File.Exists(filePath))
        {
            return null;
        }
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<ListGroupsDto.GroupDto[]>(json);
    }

    private static async Task BackupGroups(ListGroupsDto.GroupDto[] groups)
    {
        var json = JsonSerializer.Serialize(groups, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(GetBackupFilePath(BackupGroupsFilePath), json);
    }

    private static async Task<ListUsersDto.UserDto[]> GetEmployees(ZendeskApiClient zendeskApiClient)
    {
        var employees = await TryGetEmployeesFromBackup();
        if (employees is null)
        {
            AnsiConsole.MarkupLine("[yellow]No employees backup found, fetching from Zendesk.[/]");
            employees = await zendeskApiClient.GetEmployees();
            AnsiConsole.MarkupLine($"[green]Fetched {employees.Length} employees from Zendesk.[/]");
            await BackupEmployees(employees);
            AnsiConsole.MarkupLine("[green]Employees backup created.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[green]Employees backup found, using it.[/]");
        }
        return employees;
    }

    private static async Task<ListUsersDto.UserDto[]?> TryGetEmployeesFromBackup()
    {
        var filePath = GetBackupFilePath(BackupEmployeesFilePath);
        if (!File.Exists(filePath))
        {
            return null;
        }
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<ListUsersDto.UserDto[]>(json);
    }

    private static async Task BackupEmployees(ListUsersDto.UserDto[] employees)
    {
        var json = JsonSerializer.Serialize(employees, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(GetBackupFilePath(BackupEmployeesFilePath), json);
    }

    private static async Task<ListTicketsDto.TicketDto[]> GetTickets(ZendeskApiClient zendeskApiClient)
    {
        var tickets = await TryGetTicketsFromBackup();
        if (tickets is null)
        {
            AnsiConsole.MarkupLine("[yellow]No tickets backup found, fetching from Zendesk.[/]");
            tickets = await zendeskApiClient.GetTickets(100);
            AnsiConsole.MarkupLine($"[green]Fetched {tickets.Length} tickets from Zendesk.[/]");
            await BackupTickets(tickets);
            AnsiConsole.MarkupLine("[green]Tickets backup created.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[green]Tickets backup found, using it.[/]");
        }
        return tickets;
    }

    private static async Task<ListTicketsDto.TicketDto[]?> TryGetTicketsFromBackup()
    {
        var filePath = GetBackupFilePath(BackupTicketsFilePath);
        if (!File.Exists(filePath))
        {
            return null;
        }
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<ListTicketsDto.TicketDto[]>(json);
    }

    private static async Task BackupTickets(ListTicketsDto.TicketDto[] tickets)
    {
        var json = JsonSerializer.Serialize(tickets, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(GetBackupFilePath(BackupTicketsFilePath), json);
    }
    
    private static async Task<ListTicketCommentsDto.CommentDto[]> GetComments(ZendeskApiClient zendeskApiClient, long ticketId)
    {
        var comments = await TryGetCommentsFromBackup(ticketId);
        if (comments is null)
        {
            AnsiConsole.MarkupLine($"[yellow]No comments backup found for ticket {ticketId}, fetching from Zendesk.[/]");
            comments = await zendeskApiClient.GetTicketComments(ticketId);
            AnsiConsole.MarkupLine($"[green]Fetched {comments.Length} comments from Zendesk for ticket {ticketId}.[/]");
            await BackupComments(comments, ticketId);
            AnsiConsole.MarkupLine($"[green]Comments backup created for ticket {ticketId}.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[green]Comments backup found for ticket {ticketId}, using it.[/]");
        }
        return comments;
    }

    private static async Task<ListTicketCommentsDto.CommentDto[]?> TryGetCommentsFromBackup(long ticketId)
    {
        var fileName = string.Format(BackupCommentsFilePath, ticketId);
        var filePath = GetBackupFilePath(fileName);
        if (!File.Exists(filePath))
        {
            return null;
        }
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<ListTicketCommentsDto.CommentDto[]>(json);
    }

    private static async Task BackupComments(ListTicketCommentsDto.CommentDto[] comments, long ticketId)
    {
        var fileName = string.Format(BackupCommentsFilePath, ticketId);
        var json = JsonSerializer.Serialize(comments, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(GetBackupFilePath(fileName), json);
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
}