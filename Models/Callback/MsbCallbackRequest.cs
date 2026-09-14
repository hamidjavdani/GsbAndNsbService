using System.Text.Json.Serialization;

namespace GSB.Test.Api.Models.Callback;

public class MsbCallbackRequest
{
    [JsonPropertyName("organId")]
    public string OrganId { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("owTrakingCode")]
    public string OwTrakingCode { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public MsbCallbackData? Data { get; set; }

    [JsonPropertyName("error")]
    public MsbCallbackError? Error { get; set; }
}
