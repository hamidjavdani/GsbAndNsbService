using System.Text.Json.Serialization;

namespace GSB.Test.Api.Models.Responses;

public class GsbResponse
{
    [JsonPropertyName("data")]
    public string Data { get; set; } = string.Empty;
}