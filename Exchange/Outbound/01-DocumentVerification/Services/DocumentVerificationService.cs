using GSB.Test.Api.Configurations;
using GSB.Test.Api.Models.Requests;
using GSB.Test.Api.Models.Responses;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace GSB.Test.Api.Services;

public class DocumentVerificationService : IDocumentVerificationService
{
    private readonly MsbOutboundInvocation _outbound;
    private readonly MsbSettings _settings;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public DocumentVerificationService(
        IHttpClientFactory httpClientFactory,
        IOptions<MsbSettings> options,
        IMsbInvocationLogger invocationLogger, ILogger<DocumentVerificationService> diagnostics,
        IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _settings = options.Value;
        _outbound = new MsbOutboundInvocation(httpClientFactory.CreateClient("MSB"), _settings,
            invocationLogger, diagnostics, configuration, httpContextAccessor);
    }

    public async Task<G2GInquiryResponse> G2GInquiryAsync(G2GInquiryRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        MsbRequestValidation.Required(request.RequesterName, "requesterName");
        MsbRequestValidation.Required(request.RequesterFamily, "requesterFamily");
        MsbRequestValidation.Required(request.RequesterNationalCode, "requesterNationalCode");
        MsbRequestValidation.Required(request.RequesterOfficProvinceName, "requesterOfficProvinceName");
        MsbRequestValidation.Required(request.RequesterOfficeCode, "requesterOfficeCode");
        MsbRequestValidation.Required(request.RequesterOfficNumber, "requesterOfficNumber");
        MsbRequestValidation.Required(request.RequestUniqueId, "requestUniqueId");
        MsbRequestValidation.Required(request.ElectronicEstateNoteNo, "ElectronicEstateNoteNo");
        MsbRequestValidation.Required(request.NationalityCode, "nationalitycode");
        request.RuleId = MsbRequestValidation.Rule(request.RuleId, "mhrne7iv");

        var json = JsonSerializer.Serialize(request, _jsonOptions);
        return await _outbound.SendAsync("DocumentVerification", _settings.InquiryEndpoint, json,
            responseContent => JsonSerializer.Deserialize<G2GInquiryResponse>(responseContent,
                   _jsonOptions)
               ?? new G2GInquiryResponse
               {
                   Code = -1,
                   Msg = "Response is null."
               });
    }
}
