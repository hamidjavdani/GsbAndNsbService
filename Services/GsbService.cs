using GSB.Test.Api.Configurations;
using GSB.Test.Api.Models.Requests;
using GSB.Test.Api.Models.Responses;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace GSB.Test.Api.Services;

public class GsbService : IGsbService
{
    private readonly HttpClient _httpClient;
    private readonly GsbSettings _settings;
    private readonly ILogger<GsbService> _logger;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public GsbService(
        IHttpClientFactory httpClientFactory,
        IOptions<GsbSettings> options,
        ILogger<GsbService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("GSB");
        _settings = options.Value;
        _logger = logger;
    }

    public async Task<G2GInquiryResponse> G2GInquiryAsync(G2GInquiryRequest request)
    {
        try
        {
            _logger.LogInformation("GSB Inquiry started.");

            // اگر RuleId در Request مقدار نداشته باشد،
            // از appsettings مقداردهی می‌شود.
            if (string.IsNullOrWhiteSpace(request.RuleId))
            {
                request.RuleId = _settings.RuleId;
            }

            // Serialize Request
            var requestJson = JsonSerializer.Serialize(request, _jsonOptions);

            _logger.LogInformation("Request serialized successfully.");

            // Encrypt Request
            var encryptedData = EncryptionService.Encrypt(
                requestJson,
                _settings.SecretKey,
                _settings.IV);

            _logger.LogInformation("Request encrypted successfully.");

            // Create GSB Request
            var gsbRequest = new GsbRequest
            {
                ApiKey = _settings.ApiKey,
                Data = encryptedData
            };

            // Send Request
            return await SendRequestAsync(gsbRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while calling GSB service.");
            throw;
        }
    }

    private async Task<G2GInquiryResponse> SendRequestAsync(GsbRequest request)
    {
        var json = JsonSerializer.Serialize(request, _jsonOptions);

        using var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        _httpClient.DefaultRequestHeaders.Clear();

        _httpClient.DefaultRequestHeaders.Add(
            "User_Key",
            _settings.KeyUser);

        _httpClient.DefaultRequestHeaders.Add(
            "Bundle-Id",
            _settings.BundleId);

        _logger.LogInformation("Sending request to GSB...");

        var response = await _httpClient.PostAsync(
            _settings.BaseUrl,
            content);

        _logger.LogInformation(
            "GSB Response Status : {StatusCode}",
            response.StatusCode);

        _ = response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();

        _logger.LogInformation("Response received successfully.");

        var result = JsonSerializer.Deserialize<G2GInquiryResponse>(
            responseContent,
            _jsonOptions);

        return result ?? new G2GInquiryResponse
        {
            Code = -1,
            Msg = "Response is null."
        };
    }
}