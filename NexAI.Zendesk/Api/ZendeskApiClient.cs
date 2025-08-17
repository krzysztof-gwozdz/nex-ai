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
        await GetCount("/api/v2/tickets/count", "Failed to get ticket count");

    public async Task<ListTicketsDto.TicketDto[]> GetTickets(int limit) =>
        await GetPagedItems<ListTicketsDto, ListTicketsDto.TicketDto>(
            "/api/v2/tickets",
            "Failed to get tickets",
            dto => dto.Tickets,
            limit);

    public async Task<int> GetAgentsCount() => 
        await GetCount("/api/v2/users/count?role=agent", "Failed to get users count");

    public async Task<ListUsersDto.UserDto[]> GetAgents(int limit) =>
        await GetPagedItems<ListUsersDto, ListUsersDto.UserDto>(
            "/api/v2/users?role=agent",
            "Failed to get users",
            dto => dto.Users,
            limit);

    public async Task<ListTicketCommentsDto.CommentDto[]> GetTicketComments(long ticketId, int limit) =>
        await GetPagedItems<ListTicketCommentsDto, ListTicketCommentsDto.CommentDto>(
            $"/api/v2/tickets/{ticketId}/comments",
            "Failed to get ticket comments",
            dto => dto.Comments,
            limit);

    private async Task<int> GetCount(string endpoint, string errorMessage)
    {
        var response = await _httpClient.GetAsync(endpoint);
        if (!response.IsSuccessStatusCode)
            throw new($"{errorMessage}: {response.ReasonPhrase}");
        var countsDto = await ParseContentToDto<CountsDto>(response);
        return countsDto.Count?.Value ?? 0;
    }

    private async Task<TItem[]> GetPagedItems<TDto, TItem>(string endpoint, string errorMessage, Func<TDto, TItem[]?> getItems, int limit) where TDto : PagedDto
    {
        var allItems = new List<TItem>();
        var page = 1;
        var hasMorePages = true;

        while (hasMorePages && allItems.Count < limit)
        {
            var separator = endpoint.Contains('?') ? "&" : "?";
            var response = await _httpClient.GetAsync($"{endpoint}{separator}page={page}");
            if (!response.IsSuccessStatusCode)
                throw new($"{errorMessage}: {response.ReasonPhrase}");

            var dto = await ParseContentToDto<TDto>(response);
            var items = getItems(dto);
            if (items is not null)
            {
                var remainingSlots = limit - allItems.Count;
                var itemsToAdd = items.Take(remainingSlots).ToArray();
                allItems.AddRange(itemsToAdd);
            }

            hasMorePages = !string.IsNullOrEmpty(dto.NextPage) && allItems.Count < limit;
            page++;
        }

        return allItems.ToArray();
    }

    private static async Task<T> ParseContentToDto<T>(HttpResponseMessage response) =>
        await response.Content.ReadFromJsonAsync<T>() ?? throw new JsonException($"Failed to deserialize response to {typeof(T).Name}");
}