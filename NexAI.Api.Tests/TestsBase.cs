using System.Text;
using System.Text.Json;

namespace NexAI.Api.Tests;

public abstract class TestsBase(NexAIApiApplicationFactory factory) : IClassFixture<NexAIApiApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    protected Task<HttpResponseMessage> Get(string url) => _client.GetAsync(url);

    protected async Task<HttpResponseMessage> Post<TRequest>(string url, TRequest request)
    {
        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await _client.PostAsync(url, content);
    }

    protected async Task<TResponse> GetResponse<TResponse>(HttpResponseMessage response) where TResponse : class
    {
        var content = await response.Content.ReadAsStringAsync();
        return typeof(TResponse) == typeof(string)
            ? (content as TResponse)!
            : JsonSerializer.Deserialize<TResponse>(content)
              ?? throw new JsonException($"Failed to deserialize response to {typeof(TResponse).Name}");
    }
}