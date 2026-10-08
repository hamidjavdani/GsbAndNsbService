using System.Text.Json.Serialization;

namespace GSB.Test.Api.Models.Responses;

public class PoaInquiryResponse
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("msg")]
    public string Msg { get; set; } = string.Empty;

    [JsonPropertyName("owTrakingCode")]
    public string? OwTrakingCode { get; set; }
}
