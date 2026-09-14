using System.Text.Json.Serialization;

namespace GSB.Test.Api.Models.Callback;

public class RegistrationCallbackData
{
    [JsonPropertyName("GetKmlPolygonInfo")]
    public string? GetKmlPolygonInfo { get; set; }

    [JsonPropertyName("ConfirmDocumentByElectronicInfo")]
    public ConfirmDocumentByElectronicInfo? ConfirmDocumentByElectronicInfo { get; set; }

    [JsonPropertyName("ConfirmDocumentInfo")]
    public ConfirmDocumentInfo? ConfirmDocumentInfo { get; set; }
}