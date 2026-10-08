using System.Text.Json.Serialization;

namespace GSB.Test.Api.Models.Requests;

public class PoaInquiryRequest
{
    [JsonPropertyName("nationalRegisterNo")]
    public string NationalRegisterNo { get; set; } = string.Empty;

    [JsonPropertyName("secretNo")]
    public string SecretNo { get; set; } = string.Empty;

    [JsonPropertyName("requesterName")]
    public string RequesterName { get; set; } = string.Empty;

    [JsonPropertyName("requesterFamily")]
    public string RequesterFamily { get; set; } = string.Empty;

    [JsonPropertyName("requesterNationalCode")]
    public string RequesterNationalCode { get; set; } = string.Empty;

    [JsonPropertyName("requesterOfficProvinceName")]
    public string RequesterOfficProvinceName { get; set; } = string.Empty;

    [JsonPropertyName("requesterOfficeCode")]
    public string RequesterOfficeCode { get; set; } = string.Empty;

    [JsonPropertyName("requesterOfficNumber")]
    public string RequesterOfficNumber { get; set; } = string.Empty;

    [JsonPropertyName("ruleId")]
    public string RuleId { get; set; } = string.Empty;

    [JsonPropertyName("requestUniqueId")]
    public string RequestUniqueId { get; set; } = string.Empty;
}
