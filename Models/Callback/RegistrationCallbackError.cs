using System.Text.Json.Serialization;

namespace GSB.Test.Api.Models.Callback;

public class RegistrationCallbackError
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("msg")]
    public string? Message { get; set; }
}
