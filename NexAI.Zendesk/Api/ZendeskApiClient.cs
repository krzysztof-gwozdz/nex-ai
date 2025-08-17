using System.Net.Http.Json;
using System.Text.Json;
using NexAI.Config;

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

    public async Task<int> GetTicketCount() =>
        await GetCount("/api/v2/tickets/count");

    public async Task<ListTicketsDto.TicketDto[]> GetTickets(int? limit = null) =>
        await GetPagedItems<ListTicketsDto, ListTicketsDto.TicketDto>(
            "/api/v2/tickets",
            dto => dto.Tickets,
            limit);

    public async Task<int> GetEmployeesCount() =>
        await GetCount("/api/v2/users/count?role[]=agent&role[]=admin");

    public async Task<ListUsersDto.UserDto[]> GetEmployees(int? limit = null) =>
        await GetPagedItems<ListUsersDto, ListUsersDto.UserDto>(
            "/api/v2/users?role[]=agent&role[]=admin",
            dto => dto.Users,
            limit);

    public async Task<ListTicketCommentsDto.CommentDto[]> GetTicketComments(long ticketId, int? limit = null) =>
        await GetPagedItems<ListTicketCommentsDto, ListTicketCommentsDto.CommentDto>(
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