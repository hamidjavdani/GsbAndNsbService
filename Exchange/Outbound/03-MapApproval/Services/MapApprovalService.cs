using GSB.Test.Api.Configurations;
using GSB.Test.Api.Models.Requests;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace GSB.Test.Api.Services;

public class MapApprovalService : IMapApprovalService
{
    private readonly HttpClient _httpClient;
    private readonly MsbSettings _settings;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public MapApprovalService(
        IHttpClientFactory httpClientFactory,
        IOptions<MsbSettings> options)
    {
        _httpClient = httpClientFactory.CreateClient("MSB");
        _settings = options.Value;
    }

    public async Task<JsonElement> SendAsync(MapApprovalRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!request.Elzam14)
            throw new ArgumentException("elzam14 must be true.", nameof(request));

        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var url = $"{_settings.BaseUrl.TrimEnd('/')}/{_settings.MapApprovalEndpoint.TrimStart('/')}";

        using var message = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        message.Headers.Add(_settings.ApiKeyHeaderName, _settings.ApiKey);

        using var response = await _httpClient.SendAsync(message);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(
                $"MSB map approval HTTP {(int)response.StatusCode}: {responseContent}");

        using var document = JsonDocument.Parse(responseContent);
        return document.RootElement.Clone();
    }
}
