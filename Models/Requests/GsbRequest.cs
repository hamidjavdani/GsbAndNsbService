
using System.Text.Json.Serialization;

namespace GSB.Test.Api.Models.Requests;

public class GsbRequest
{
    /// <summary>
    /// کلید شناسایی سازمان
    /// </summary>
    [JsonPropertyName("apiKey")]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// داده رمزنگاری شده
    /// </summary>
    [JsonPropertyName("data")]
    public string Data { get; set; } = string.Empty;
}