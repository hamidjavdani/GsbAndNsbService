using System.Text.Json.Serialization;

namespace GSB.Test.Api.Models.Callback;

public class RegistrationCallbackError
{
    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; set; }

    [JsonPropertyName("errorCode")]
    public int? ErrorCode { get; set; }
}
