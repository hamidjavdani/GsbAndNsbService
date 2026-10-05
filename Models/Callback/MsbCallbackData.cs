using System.Text.Json;
using System.Text.Json.Serialization;

namespace GSB.Test.Api.Models.Callback;

public class MsbCallbackData
{
    [JsonPropertyName("GetKmlPolygonInfo")]
    public JsonElement? GetKmlPolygonInfo { get; set; }

    [JsonPropertyName("ConfirmDocumentByElectronicInfo")]
    public ConfirmDocumentByElectronicInfo? ConfirmDocumentByElectronicInfo { get; set; }

    [JsonPropertyName("ConfirmDocumentInfo")]
    public ConfirmDocumentInfo? ConfirmDocumentInfo { get; set; }
}
