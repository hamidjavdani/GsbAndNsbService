namespace GSB.Test.Api.Configurations;

public class MsbSettings
{
    public string BaseUrl { get; set; } = "http://pmsbgw.shahr-bank.ir";
    public string InquiryEndpoint { get; set; } = "/interagency/document-verification-inquiry/G2GInquery";
    public string PoaInquiryEndpoint { get; set; } = "/interagency/poa/G2GInquery";
    public string CancellationEndpoint { get; set; } = "/made14/ebtal/v1/response";
    public string ApiKeyHeaderName { get; set; } = "X-MSB-Api-Key";
    public string ApiKey { get; set; } = string.Empty;
    public string RuleId { get; set; } = "mhrne7iv";
    public string PoaInquiryRuleId { get; set; } = "mhlu20po";
}
