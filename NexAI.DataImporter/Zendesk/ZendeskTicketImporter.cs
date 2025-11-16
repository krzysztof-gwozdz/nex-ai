using System.Text.Json;
using System.Collections.Concurrent;
using MongoDB.Driver;
using NexAI.Config;
using NexAI.MongoDb;
using NexAI.RabbitMQ;
using NexAI.Zendesk;
using NexAI.Zendesk.Api;
using NexAI.Zendesk.Api.Dtos;
using NexAI.Zendesk.MongoDb;
using Spectre.Console;

namespace NexAI.DataImporter.Zendesk;

internal class ZendeskTicketImporter(ZendeskApiClient zendeskApiClient, RabbitMQClient rabbitMQClient, MongoDbClient mongoDbClient, Options options)
{
    private const string BackupEmployeesFilePath = "zendesk_api_backup_employees.json";
    private const string BackupTicketsFilePath = "zendesk_api_backup_tickets_{0:yyyyMMdd}.json";
    private const string BackupCommentsFilePath = "zendesk_api_backup_{0}_{1}_comments.json";

    public async Task Import(CancellationToken cancellationToken)
    {
        AnsiConsole.MarkupLine("[yellow]Importing Zendesk tickets from API...[/]");
        var employees = await GetEmployeesFromApiOrBackup(cancellationToken);
        var tickets = await GetTicketsFromApiOrBackup(GetTicketStartDateTime(), cancellationToken);
        var ticketsCount = 0;
        const int batchSize = 100;
        for (var i = 0; i < tickets.Length; i += batchSize)
        {
            var zendeskTickets = new ConcurrentBag<ZendeskTicket>();
            await Parallel.ForEachAsync(tickets.Skip(i).Take(batchSize),
                new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
                async (ticket, parallelCancellationToken) =>
                {
                    var comments = await GetCommentsFromApiOrBackup(ticket.Id!.Value, ticket.UpdatedAt, parallelCancellationToken);
                    var mappedZendeskTicket = ZendeskTicketMapper.Map(ticket, comments, employees);
                    if (mappedZendeskTicket.IsRelevant)
                    {
                        zendeskTickets.Add(mappedZendeskTicket);
                        Interlocked.Increment(ref ticketsCount);
                    }
                });
            var zendeskTicketImportedEvents = zendeskTickets.Select(zendeskTicket => zendeskTicket.ToZendeskTicketImportedEvent()).ToArray();
            await rabbitMQClient.Send(RabbitMQStructure.ZendeskTicketExchangeName, zendeskTicketImportedEvents, cancellationToken);
        }
        AnsiConsole.MarkupLine($"[green]Imported {tickets.Length} Zendesk tickets. Only {ticketsCount} were relevant.[/]");
    }

    private async Task<UserDto[]> GetEmployeesFromApiOrBackup(CancellationToken cancellationToken) =>
        await GetFromApiOrBackup(
            BackupEmployeesFilePath,
            () => zendeskApiClient.GetEmployees(null, cancellationToken),
            "Employees",
            user => $"Fetched {user.Length} employees from Zendesk.",
            cancellationToken);

    private async Task<TicketDto[]> GetTicketsFromApiOrBackup(DateTime startTime, CancellationToken cancellationToken) =>
        await GetFromApiOrBackup(
            string.Format(BackupTicketsFilePath, startTime),
            () => zendeskApiClient.GetTickets(startTime, cancellationToken),
            "Tickets",
            ticket => $"Fetched {ticket.Length} tickets from Zendesk.",
            cancellationToken);

    private async Task<CommentDto[]> GetCommentsFromApiOrBackup(long ticketId, string? updatedAt, CancellationToken cancellationToken) =>
        await GetFromApiOrBackup(
            string.Format(BackupCommentsFilePath, ticketId, updatedAt?.Replace(":", "-")),
            () => zendeskApiClient.GetTicketComments(ticketId, null, cancellationToken),
            $"Comments for ticket {ticketId}",
            comment => $"Fetched {comment.Length} comments from Zendesk for ticket {ticketId}.",
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

    private DateTime GetTicketStartDateTime()
    {
        var lastImportDate = mongoDbClient.Database.GetCollection<ZendeskTicketMongoDbDocument>(ZendeskTicketMongoDbCollection.Name)
            .Find(FilterDefinition<ZendeskTicketMongoDbDocument>.Empty)
            .SortByDescending(document => document.LastImportDate)
            .Limit(1)
            .SingleOrDefault()?.LastImportDate;
        if (lastImportDate is null)
        {
            var startDate = options.Get<DataImporterOptions>().ZendeskTicketStartDate;
            AnsiConsole.MarkupLine($"[yellow]No previous import found. Fetching tickets updated since configured start date {startDate:yyyy-MM-dd HH:mm:ss}.[/]");
            return startDate;
        }
        AnsiConsole.MarkupLine($"[yellow]Fetching tickets updated since {lastImportDate:yyyy-MM-dd HH:mm:ss}.[/]");
        return lastImportDate.Value;
    }
}