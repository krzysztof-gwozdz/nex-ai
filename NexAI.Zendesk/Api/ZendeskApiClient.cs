using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using NexAI.Zendesk.Api.Dtos;

namespace NexAI.Zendesk.Api;

public class ZendeskApiClient(ILogger<ZendeskApiClient> logger, HttpClient httpClient)
{
    public async Task<int> GetEmployeesCount(CancellationToken cancellationToken) =>
        await GetCount("/api/v2/users/count?role[]=agent&role[]=admin&exclude_deleted=true", cancellationToken);

    public async Task<int> GetGroupsCount(CancellationToken cancellationToken) =>
        await GetCount("/api/v2/groups/count?exclude_deleted=true", cancellationToken);

    public async Task<int> GetTicketsCount(CancellationToken cancellationToken) =>
        await GetCount("/api/v2/tickets/count?exclude_deleted=true", cancellationToken);

    public async Task<UserDto[]> GetEmployees(int? limit, CancellationToken cancellationToken) =>
        await GetPagedItems<ListUsersDto, UserDto>(
            "/api/v2/users?role[]=agent&role[]=admin&exclude_deleted=true",
            dto => dto.Users,
            limit,
            cancellationToken);

    public async Task<GroupDto[]> GetGroups(int? limit, CancellationToken cancellationToken) =>
        await GetPagedItems<ListGroupsDto, GroupDto>(
            "/api/v2/groups?exclude_deleted=true",
            dto => dto.Groups,
            limit,
            cancellationToken);

    public async Task<GroupDto[]> GetUserGroups(long userId, int? limit, CancellationToken cancellationToken) =>
        await GetPagedItems<ListGroupsDto, GroupDto>(
            $"/api/v2/users/{userId}/groups?exclude_deleted=true",
            dto => dto.Groups,
            limit,
            cancellationToken);

    public async Task<TicketDto> GetTicket(string id, CancellationToken cancellationToken) =>
        (await Get<GetTicketDto>($"/api/v2/tickets/{id}", cancellationToken)).Ticket ?? throw new($"Ticket with id {id} not found.");

    public async Task<TicketDto[]> GetTickets(DateTime startTime, CancellationToken cancellationToken)
    {
        var timestamp = ((DateTimeOffset)startTime).ToUnixTimeSeconds();
        var tickets = new List<TicketDto>();
        bool endOfStream;
        do
        {
            var endpoint = $"/api/v2/incremental/tickets?exclude_deleted=true&start_time={timestamp}";
            logger.LogInformation("Fetching page from {endpoint} ({time:dd/MM/yyyy})", endpoint, DateTimeOffset.FromUnixTimeSeconds(timestamp));
            var response = await httpClient.GetAsync(endpoint, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new($"Failed to get items from {endpoint}: {response.ReasonPhrase}");
            }
            var dto = await ParseContentToDto<IncrementalTicketExportDto>(response);
            if (dto.Tickets is not null)
            {
                tickets.AddRange(dto.Tickets);
            }
            logger.LogInformation("Fetched {Count} tickets in total.", tickets.Count);
            endOfStream = dto.EndOfStream ?? false;
            timestamp = dto.EndTime ?? long.MaxValue;
        } while (!endOfStream);
        return tickets.DistinctBy(ticket => ticket.Id).ToArray();
    }

    public async Task<TicketDto[]> GetTickets(int? limit, CancellationToken cancellationToken) =>
        await GetPagedItems<ListTicketsDto, TicketDto>(
            "/api/v2/tickets",
            dto => dto.Tickets,
            limit,
            cancellationToken);

    public async Task<CommentDto[]> GetTicketComments(long ticketId, int? limit, CancellationToken cancellationToken) =>
        await GetPagedItems<ListTicketCommentsDto, CommentDto>(
            $"/api/v2/tickets/{ticketId}/comments",
            dto => dto.Comments,
            limit,
            cancellationToken);

    private async Task<int> GetCount(string endpoint, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching page count from {endpoint}", endpoint);
        var response = await httpClient.GetAsync(endpoint, cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new($"Failed to get items from {endpoint}: {response.ReasonPhrase}");
        var countsDto = await ParseContentToDto<CountsDto>(response);
        return countsDto.Count?.Value ?? 0;
    }

    private async Task<TDto> Get<TDto>(string endpoint, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching data from {endpoint}", endpoint);
        var response = await httpClient.GetAsync(endpoint, cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new($"Failed to get items from {endpoint}: {response.ReasonPhrase}");
        var dto = await ParseContentToDto<TDto>(response);
        return dto;
    }

    private async Task<TItem[]> GetPagedItems<TDto, TItem>(string endpoint, Func<TDto, TItem[]?> getItems, int? limit, CancellationToken cancellationToken) where TDto : PagedDto
    {
        var allItems = new List<TItem>();
        var page = 1;
        var hasMorePages = true;

        while (hasMorePages)
        {
            var separator = endpoint.Contains('?') ? "&" : "?";
            var url = $"{endpoint}{separator}page={page}";
            logger.LogInformation("Fetching page {Page} from {Url}", page, url);
            var response = await httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new($"Failed to get items from {endpoint}: {response.ReasonPhrase}");
            }
            var dto = await ParseContentToDto<TDto>(response);
            var items = getItems(dto);
            if (items is not null)
            {
                var remainingSlots = limit.HasValue ? limit.Value - allItems.Count : items.Length;
                var itemsToAdd = items.Take(remainingSlots).ToArray();
                allItems.AddRange(itemsToAdd);
            }

            hasMorePages = !string.IsNullOrEmpty(dto.NextPage) && (limit is null || allItems.Count < limit);
            page++;
        }

        return allItems.ToArray();
    }

    private static async Task<T> ParseContentToDto<T>(HttpResponseMessage response) =>
        await response.Content.ReadFromJsonAsync<T>() ?? throw new JsonException($"Failed to deserialize response to {typeof(T).Name}");
}