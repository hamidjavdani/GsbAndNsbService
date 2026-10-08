using System.Text.Json.Serialization;

namespace GSB.Test.Api.Models.Responses;

public class Made14CancellationResponse
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("msg")]
    public string Msg { get; set; } = string.Empty;
}
