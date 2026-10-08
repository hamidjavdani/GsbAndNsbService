using System.Text.Json.Serialization;

namespace GSB.Test.Api.Models.Callback;

/// <summary>
/// پاسخ وضعیت ثبت در فرایند ماده 14 که از MSB به شهرداری ارسال می‌شود.
/// </summary>
public class RegistrationStatusCallbackRequest
{
    [JsonPropertyName("organId")]
    public string? OrganId { get; set; }

    [JsonPropertyName("owTrakingCode")]
    public string? OwTrakingCode { get; set; }

    [JsonPropertyName("sabtTrackingCode")]
    public string? SabtTrackingCode { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("result")]
    public RegistrationStatusResult Result { get; set; } = new();
}

public class RegistrationStatusResult
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("msg")]
    public string? Message { get; set; }
}
