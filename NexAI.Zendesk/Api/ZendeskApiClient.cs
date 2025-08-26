using System.Net.Http.Json;
using System.Text.Json;
using NexAI.Config;
using NexAI.Zendesk.Api.Dtos;

namespace NexAI.Zendesk.Api;

public class ZendeskApiClient
{
    private readonly HttpClient _httpClient;

    public ZendeskApiClient(Options options)
    {
        var zendeskOptions = options.Get<ZendeskOptions>();
        _httpClient = new()
        {
            BaseAddress = new(zendeskOptions.ApiBaseUrl)
        };
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {zendeskOptions.AuthorizationToken}");
    }

    public async Task<GroupDto[]> GetGroups() =>
        await GetPagedItems<ListGroupsDto, GroupDto>(
            "/api/v2/groups",
            dto => dto.Groups,
            null);

    public async Task<int> GetTicketCount() =>
        await GetCount("/api/v2/tickets/count");

    public async Task<TicketDto[]> GetTickets(DateTime startTime)
    {
        var timestamp = ((DateTimeOffset)startTime).ToUnixTimeSeconds();
        var tickets = new List<TicketDto>();
        bool endOfStream;
        do
        {
            var endpoint = $"/api/v2/incremental/tickets?exclude_deleted=true&start_time={timestamp}";
            var response = await _httpClient.GetAsync(endpoint);
            if (!response.IsSuccessStatusCode)
                throw new($"Failed to get items from {endpoint}: {response.ReasonPhrase}");
            var dto = await ParseContentToDto<IncrementalTicketExportDto>(response);
            if (dto.Tickets is not null)
            {
                tickets.AddRange(dto.Tickets);
            }
            endOfStream = dto.EndOfStream ?? false;
            timestamp = dto.EndTime ?? long.MaxValue;
        } while (!endOfStream);
        return tickets.DistinctBy(ticket => ticket.Id).ToArray();
    }

    public async Task<TicketDto[]> GetTickets(int? limit = null) =>
        await GetPagedItems<ListTicketsDto, TicketDto>(
            "/api/v2/tickets",
            dto => dto.Tickets,
            limit);

    public async Task<int> GetEmployeesCount() =>
        await GetCount("/api/v2/users/count?role[]=agent&role[]=admin");

    public async Task<UserDto[]> GetEmployees(int? limit = null) =>
        await GetPagedItems<ListUsersDto, UserDto>(
            "/api/v2/users?role[]=agent&role[]=admin",
            dto => dto.Users,
            limit);

    public async Task<CommentDto[]> GetTicketComments(long ticketId, int? limit = null) =>
        await GetPagedItems<ListTicketCommentsDto, CommentDto>(
            $"/api/v2/tickets/{ticketId}/comments",
            dto => dto.Comments,
            limit);

    private async Task<int> GetCount(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);
        if (!response.IsSuccessStatusCode)
            throw new($"Failed to get items from {endpoint}: {response.ReasonPhrase}");
        var countsDto = await ParseContentToDto<CountsDto>(response);
        return countsDto.Count?.Value ?? 0;
    }

    private async Task<TItem[]> GetPagedItems<TDto, TItem>(string endpoint, Func<TDto, TItem[]?> getItems, int? limit) where TDto : PagedDto
    {
        var allItems = new List<TItem>();
        var page = 1;
        var hasMorePages = true;

        while (hasMorePages)
        {
            var separator = endpoint.Contains('?') ? "&" : "?";
            var response = await _httpClient.GetAsync($"{endpoint}{separator}page={page}");
            if (!response.IsSuccessStatusCode)
                throw new($"Failed to get items from {endpoint}: {response.ReasonPhrase}");

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