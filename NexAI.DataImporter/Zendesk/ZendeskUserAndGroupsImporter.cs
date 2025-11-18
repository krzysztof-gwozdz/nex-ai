using System.Text.Json;
using System.Collections.Concurrent;
using NexAI.Config;
using NexAI.ServiceBus;
using NexAI.Zendesk;
using NexAI.Zendesk.Api;
using NexAI.Zendesk.Api.Dtos;
using Spectre.Console;

namespace NexAI.DataImporter.Zendesk;

internal class ZendeskUserAndGroupsImporter(ZendeskApiClient zendeskApiClient, RabbitMQClient rabbitMQClient, Options options)
{
    private const string BackupGroupsFilePath = "zendesk_api_backup_groups.json";
    private const string BackupEmployeesFilePath = "zendesk_api_backup_employees.json";
    private const string BackupUserGroupsFilePath = "zendesk_api_backup_{0}_groups.json";

    public async Task Import(CancellationToken cancellationToken)
    {
        var groups = await ImportGroups(cancellationToken);
        var users = await ImportUsers(cancellationToken);
        await ImportUserGroups(groups, users, cancellationToken);
    }

    private async Task<ZendeskGroup[]> ImportGroups(CancellationToken cancellationToken)
    {
        AnsiConsole.MarkupLine("[yellow]Importing Zendesk groups from API...[/]");
        var groups = await GetGroupsFromApiOrBackup(cancellationToken);
        var zendeskGroups = groups.Select(ZendeskGroupMapper.Map).ToArray();
        var zendeskGroupImportedEvents = zendeskGroups.Select(zendeskGroup => zendeskGroup.ToZendeskGroupImportedEvent()).ToArray();
        await rabbitMQClient.Send(RabbitMQStructure.ZendeskGroupExchangeName, zendeskGroupImportedEvents, cancellationToken);
        AnsiConsole.MarkupLine($"[green]Imported {groups.Length} Zendesk groups.[/]");
        return zendeskGroups;
    }

    private async Task<ZendeskUser[]> ImportUsers(CancellationToken cancellationToken)
    {
        AnsiConsole.MarkupLine("[yellow]Importing Zendesk users from API...[/]");
        var employees = await GetEmployeesFromApiOrBackup(cancellationToken);
        var zendeskUsers = employees.Select(ZendeskUserMapper.Map).ToArray();
        var zendeskUserImportedEvents = zendeskUsers.Select(zendeskUser => zendeskUser.ToZendeskUserImportedEvent()).ToArray();
        await rabbitMQClient.Send(RabbitMQStructure.ZendeskUserExchangeName, zendeskUserImportedEvents, cancellationToken);
        AnsiConsole.MarkupLine($"[green]Imported {employees.Length} Zendesk users.[/]");
        return zendeskUsers;
    }

    private async Task ImportUserGroups(ZendeskGroup[] groups, ZendeskUser[] users, CancellationToken cancellationToken)
    {
        AnsiConsole.MarkupLine("[yellow]Importing Zendesk user groups from API...[/]");
        var employees = await GetEmployeesFromApiOrBackup(cancellationToken);
        var usersWithGroups = 0;
        const int batchSize = 100;
        for (var i = 0; i < employees.Length; i += batchSize)
        {
            var zendeskUserGroups = new ConcurrentBag<ZendeskUserGroups>();
            await Parallel.ForEachAsync(employees.Skip(i).Take(batchSize),
                new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
                async (employee, parallelCancellationToken) =>
                {
                    var zendeskUser = users.FirstOrDefault(u => u.ExternalId == employee.Id?.ToString());
                    if (zendeskUser is null)
                    {
                        AnsiConsole.MarkupLine($"[red]Could not find user for employee {employee.Id} - {employee.Name}[/]");
                        return;
                    }
                    var userGroups = await GetUserGroupsFromApiOrBackup(employee.Id!.Value, parallelCancellationToken);
                    var zendeskUserGroupIds = userGroups
                        .Select(ug => groups.FirstOrDefault(g => g.ExternalId == ug.Id.ToString())?.Id)
                        .OfType<ZendeskGroupId>()
                        .ToArray();
                    if (zendeskUserGroupIds.Length == 0)
                    {
                        AnsiConsole.MarkupLine($"[red]User does not have any groups in Zendesk: {employee.Id} - {employee.Name}[/]");
                        return;
                    }
                    zendeskUserGroups.Add(new(zendeskUser.Id, zendeskUserGroupIds));
                    Interlocked.Increment(ref usersWithGroups);
                });
            var zendeskUserGroupsImportedEvents = zendeskUserGroups.Select(zendeskUserGroup => zendeskUserGroup.ToZendeskUserGroupsImportedEvent()).ToArray();
            await rabbitMQClient.Send(RabbitMQStructure.ZendeskUsersAndGroupsExchangeName, zendeskUserGroupsImportedEvents, cancellationToken);
        }
        AnsiConsole.MarkupLine($"[green]Imported {employees.Length} Zendesk users. Only {usersWithGroups} have groups.[/]");
    }

    private async Task<GroupDto[]> GetGroupsFromApiOrBackup(CancellationToken cancellationToken) =>
        await GetFromApiOrBackup(
            BackupGroupsFilePath,
            () => zendeskApiClient.GetGroups(null, cancellationToken),
            "Groups",
            group => $"Fetched {group.Length} groups from Zendesk.",
            cancellationToken);

    private async Task<UserDto[]> GetEmployeesFromApiOrBackup(CancellationToken cancellationToken) =>
        await GetFromApiOrBackup(
            BackupEmployeesFilePath,
            () => zendeskApiClient.GetEmployees(null, cancellationToken),
            "Employees",
            user => $"Fetched {user.Length} employees from Zendesk.",
            cancellationToken);

    private async Task<GroupDto[]> GetUserGroupsFromApiOrBackup(long userId, CancellationToken cancellationToken) =>
        await GetFromApiOrBackup(
            string.Format(BackupUserGroupsFilePath, userId),
            () => zendeskApiClient.GetUserGroups(userId, null, cancellationToken),
            $"Groups for user {userId}",
            userGroup => $"Fetched {userGroup.Length} groups from Zendesk for user {userId}.",
            cancellationToken);

    private async Task<T[]> GetFromApiOrBackup<T>(string filename, Func<Task<T[]>> fetchFromApi, string entityDescription, Func<T[], string> fetchedMessage, CancellationToken cancellationToken)
    {
        var filePath = GetBackupFilePath(filename);
        if (options.Get<DataImporterOptions>().UseBackup && await TryLoadFromBackup<T[]>(filePath, cancellationToken) is { } backup)
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

    private static async Task<T?> TryLoadFromBackup<T>(string filePath, CancellationToken cancellationToken)
    {
        if (!File.Exists(filePath))
        {
            return default;
        }
        var json = await File.ReadAllTextAsync(filePath, cancellationToken);
        return JsonSerializer.Deserialize<T>(json);
    }

    private static async Task BackupToFile<T>(string filePath, T data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
    }
}