using System.Text.Json.Serialization;

namespace GSB.Test.Api.Models.Callback;

public class RegistrationCallbackRequest
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("data")]
    public RegistrationCallbackData? Data { get; set; }

    [JsonPropertyName("error")]
    public RegistrationCallbackError? Error { get; set; }
}
