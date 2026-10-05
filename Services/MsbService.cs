using GSB.Test.Api.Configurations;
using GSB.Test.Api.Models.Requests;
using GSB.Test.Api.Models.Responses;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace GSB.Test.Api.Services;

public class MsbService : IMsbService
{
    private readonly HttpClient _httpClient;
    private readonly MsbSettings _settings;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public MsbService(IHttpClientFactory httpClientFactory, IOptions<MsbSettings> options)
    {
        _httpClient = httpClientFactory.CreateClient("MSB");
        _settings = options.Value;
    }

    public async Task<G2GInquiryResponse> G2GInquiryAsync(G2GInquiryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RuleId))
            request.RuleId = _settings.RuleId;

        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var url = $"{_settings.BaseUrl.TrimEnd('/')}/{_settings.InquiryEndpoint.TrimStart('/')}";

        using var message = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        message.Headers.Add(_settings.ApiKeyHeaderName, _settings.ApiKey);

        using var response = await _httpClient.SendAsync(message);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"MSB HTTP {(int)response.StatusCode}: {responseContent}");

        return JsonSerializer.Deserialize<G2GInquiryResponse>(responseContent, _jsonOptions)
            ?? new G2GInquiryResponse { Code = -1, Msg = "Response is null." };
    }
}
