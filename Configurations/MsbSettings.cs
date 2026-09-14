namespace GSB.Test.Api.Configurations;

public class MsbSettings
{
    public string BaseUrl { get; set; } = "http://pmsbgw.shahr-bank.ir";
    public string InquiryEndpoint { get; set; } = "/interagency/document-verification-inquiry/G2GInquery";
    public string ApiKey { get; set; } = string.Empty;
    public string RuleId { get; set; } = "mhrne7iv";
}
