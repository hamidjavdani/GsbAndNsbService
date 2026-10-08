using GSB.Test.Api.Configurations;
using GSB.Test.Api.Models.Requests;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace GSB.Test.Api.Services;

public class MapApprovalService : IMapApprovalService
{
    private readonly MsbOutboundInvocation _outbound;
    private readonly MsbSettings _settings;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public MapApprovalService(
        IHttpClientFactory httpClientFactory,
        IOptions<MsbSettings> options,
        IMsbInvocationLogger invocationLogger, ILogger<MapApprovalService> diagnostics,
        IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _settings = options.Value;
        _outbound = new MsbOutboundInvocation(httpClientFactory.CreateClient("MSB"), _settings,
            invocationLogger, diagnostics, configuration, httpContextAccessor);
    }

    public async Task<JsonElement> SendAsync(MapApprovalRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        MapApprovalValidation.Validate(request);

        var json = JsonSerializer.Serialize(request, _jsonOptions);
        return await _outbound.SendAsync("MapApproval", _settings.MapApprovalEndpoint, json, responseContent =>
        {
            using var document = JsonDocument.Parse(responseContent);
            return document.RootElement.Clone();
        });
    }
}
