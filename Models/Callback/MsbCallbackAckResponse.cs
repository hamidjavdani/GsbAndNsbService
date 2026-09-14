using System.Text.Json.Serialization;

namespace GSB.Test.Api.Models.Callback;

public class MsbCallbackAckResponse
{
    [JsonPropertyName("msbTrackingCode")]
    public string? MsbTrackingCode { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;
}
