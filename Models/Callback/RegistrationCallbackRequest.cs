using System.Text.Json.Serialization;

namespace GSB.Test.Api.Models.Callback;

public class RegistrationCallbackRequest
{
    [JsonPropertyName("organId")]
    public string OrganId { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("owTrakingCode")]
    public string OwTrakingCode { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public RegistrationCallbackData? Data { get; set; }

    [JsonPropertyName("error")]
    public RegistrationCallbackError? Error { get; set; }
}
