using System.Text.Json.Serialization;

namespace GSB.Test.Api.Models.Requests;

public class Made14CancellationRequest
{
    [JsonPropertyName("organId")]
    public string OrganId { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public Made14CancellationData Data { get; set; } = new();

    [JsonPropertyName("owTrackingCode")]
    public string OwTrackingCode { get; set; } = string.Empty;

    [JsonPropertyName("noToken")]
    public bool NoToken { get; set; } = true;

    [JsonPropertyName("ruleId")]
    public string RuleId { get; set; } = string.Empty;
}

public class Made14CancellationData
{
    [JsonPropertyName("cancelReason")]
    public string CancelReason { get; set; } = string.Empty;

    [JsonPropertyName("actionId")]
    public string ActionId { get; set; } = string.Empty;
}

public static class Made14CancellationRuleIds
{
    // پرونده هنوز از سازمان ثبت کد رهگیری دریافت نکرده است.
    public const string WithoutRegistrationTrackingCode = "mo6mgrjz";

    // پرونده از سازمان ثبت کد رهگیری دریافت کرده است.
    public const string WithRegistrationTrackingCode = "bnhz2mgw";
}
