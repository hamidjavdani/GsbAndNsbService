using GSB.Test.Api.Configurations;
using GSB.Test.Api.Data.Entities;
using GSB.Test.Api.Models.Requests;
using GSB.Test.Api.Models.Responses;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace GSB.Test.Api.Services;

public class Made14CancellationService : IMade14CancellationService
{
    private readonly HttpClient _httpClient;
    private readonly MsbSettings _settings;
    private readonly IMsbInvocationLogger _invocationLogger;
    private readonly ILogger<Made14CancellationService> _diagnostics;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public Made14CancellationService(
        IHttpClientFactory httpClientFactory,
        IOptions<MsbSettings> options,
        IMsbInvocationLogger invocationLogger,
        ILogger<Made14CancellationService> diagnostics,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClientFactory.CreateClient("MSB");
        _settings = options.Value;
        _invocationLogger = invocationLogger;
        _diagnostics = diagnostics;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Made14CancellationResponse> CancelMade14Async(
        Made14CancellationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateCancellationRequest(request);

        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var url = $"{_settings.BaseUrl.TrimEnd('/')}/{_settings.CancellationEndpoint.TrimStart('/')}";

        using var message = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        message.Headers.Add(_settings.ApiKeyHeaderName, _settings.ApiKey);

        var stopwatch = Stopwatch.StartNew();
        var id = Guid.Empty;
        InvocationLogSanitizer? sanitizer = null;
        try
        {
            sanitizer = new InvocationLogSanitizer(_configuration, _settings.ApiKey);
            var maskedRequest = sanitizer.Body(json);
            var context = _httpContextAccessor.HttpContext;
            var target = new UriBuilder(url)
            {
                UserName = "",
                Password = "",
                Query = "",
                Fragment = ""
            };
            var log = new MsbInvocationLog
            {
                Direction = "Outbound",
                ServiceName = "Made14Cancellation",
                Endpoint = sanitizer.Text(target.Uri.GetLeftPart(UriPartial.Path))!,
                HttpMethod = "POST",
                ReceivedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Host = sanitizer.Text(target.Host),
                ContentType = "application/json",
                ContentLength = Encoding.UTF8.GetByteCount(json),
                TraceIdentifier = sanitizer.Text(context?.TraceIdentifier, 256),
                CorrelationId = sanitizer.Text(
                    context?.Request.Headers["X-Correlation-ID"].FirstOrDefault())
            };
            InvocationLogData.Request(log, maskedRequest);
            id = await SafeInvocationLogging.BeginAsync(
                _invocationLogger,
                log,
                _diagnostics);
        }
        catch (Exception ex)
        {
            _diagnostics.LogWarning(
                "Outbound invocation capture failed ({ErrorType}).",
                ex.GetType().Name);
        }

        int? httpStatus = null;
        string? responseContent = null;
        Exception? failure = null;
        try
        {
            using var response = await _httpClient.SendAsync(message);
            httpStatus = (int)response.StatusCode;
            responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(
                    $"MSB cancellation HTTP {(int)response.StatusCode}; response details omitted to protect secrets.");

            return JsonSerializer.Deserialize<Made14CancellationResponse>(
                       responseContent,
                       _jsonOptions)
                   ?? new Made14CancellationResponse
                   {
                       Code = -1,
                       Msg = "Response is null."
                   };
        }
        catch (Exception ex)
        {
            failure = ex;
            throw;
        }
        finally
        {
            try
            {
                var completion = InvocationLogData.Completion(
                    stopwatch.ElapsedMilliseconds,
                    httpStatus,
                    sanitizer?.Body(responseContent),
                    failure);
                await SafeInvocationLogging.CompleteAsync(
                    _invocationLogger,
                    id,
                    completion,
                    _diagnostics);
            }
            catch (Exception ex)
            {
                _diagnostics.LogWarning(
                    "Outbound invocation completion failed ({ErrorType}).",
                    ex.GetType().Name);
            }
        }
    }

    private static void ValidateCancellationRequest(
        Made14CancellationRequest request)
    {
        if (!request.NoToken)
            throw new ArgumentException("noToken must be true.", nameof(request));

        if (string.IsNullOrWhiteSpace(request.OrganId))
            throw new ArgumentException("organId is required.", nameof(request));

        if (request.Data is null)
            throw new ArgumentException("data is required.", nameof(request));

        if (string.IsNullOrWhiteSpace(request.Data.CancelReason))
            throw new ArgumentException(
                "data.cancelReason is required.",
                nameof(request));

        if (string.IsNullOrWhiteSpace(request.Data.ActionId))
            throw new ArgumentException(
                "data.actionId is required.",
                nameof(request));

        if (string.IsNullOrWhiteSpace(request.OwTrackingCode))
            throw new ArgumentException(
                "owTrackingCode is required.",
                nameof(request));

        if (string.IsNullOrWhiteSpace(request.RuleId))
            throw new ArgumentException("ruleId is required.", nameof(request));

        if (request.RuleId !=
                Made14CancellationRuleIds.WithoutRegistrationTrackingCode &&
            request.RuleId !=
                Made14CancellationRuleIds.WithRegistrationTrackingCode)
        {
            throw new ArgumentException(
                "ruleId is not valid for made14 cancellation.",
                nameof(request));
        }
    }
}
