using GSB.Test.Api.Configurations;
using GSB.Test.Api.Models.Requests;
using GSB.Test.Api.Models.Responses;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace GSB.Test.Api.Services;

public class PoaInquiryService : IPoaInquiryService
{
    private readonly MsbOutboundInvocation _outbound;
    private readonly MsbSettings _settings;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public PoaInquiryService(
        IHttpClientFactory httpClientFactory,
        IOptions<MsbSettings> options,
        IMsbInvocationLogger invocationLogger, ILogger<PoaInquiryService> diagnostics,
        IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _settings = options.Value;
        _outbound = new MsbOutboundInvocation(httpClientFactory.CreateClient("MSB"), _settings,
            invocationLogger, diagnostics, configuration, httpContextAccessor);
    }

    public async Task<PoaInquiryResponse> SendAsync(PoaInquiryRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        MsbRequestValidation.Required(request.NationalRegisterNo, "nationalRegisterNo");
        MsbRequestValidation.Required(request.SecretNo, "secretNo");
        MsbRequestValidation.Required(request.RequesterName, "requesterName");
        MsbRequestValidation.Required(request.RequesterFamily, "requesterFamily");
        MsbRequestValidation.Required(request.RequesterNationalCode, "requesterNationalCode");
        MsbRequestValidation.Required(request.RequesterOfficProvinceName, "requesterOfficProvinceName");
        MsbRequestValidation.Required(request.RequesterOfficeCode, "requesterOfficeCode");
        MsbRequestValidation.Required(request.RequesterOfficNumber, "requesterOfficNumber");
        MsbRequestValidation.Required(request.RequestUniqueId, "requestUniqueId");
        request.RuleId = MsbRequestValidation.Rule(request.RuleId, "mhlu20po");

        var json = JsonSerializer.Serialize(request, _jsonOptions);
        return await _outbound.SendAsync("PoaInquiry", _settings.PoaInquiryEndpoint, json,
            responseContent => JsonSerializer.Deserialize<PoaInquiryResponse>(responseContent,
                   _jsonOptions)
               ?? new PoaInquiryResponse
               {
                   Code = -1,
                   Msg = "Response is null."
               });
    }
}
