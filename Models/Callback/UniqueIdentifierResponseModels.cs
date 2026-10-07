using System.Text.Json.Serialization;

namespace GSB.Test.Api.Models.Callback;

public class UniqueIdentifierResponseRequest
{
    [JsonPropertyName("organId")]
    public string? OrganId { get; set; }

    [JsonPropertyName("data")]
    public UniqueIdentifierWorkflowData? Data { get; set; }

    [JsonPropertyName("mapConfirmationTrackingCode")]
    public string? MapConfirmationTrackingCode { get; set; }

    [JsonPropertyName("landData")]
    public List<UniqueIdentifierLandData>? LandData { get; set; }

    [JsonPropertyName("userInfo")]
    public List<UniqueIdentifierUserInfo>? UserInfo { get; set; }
}

public class UniqueIdentifierWorkflowData
{
    [JsonPropertyName("actionId")]
    public string? ActionId { get; set; }

    [JsonPropertyName("ruleId")]
    public string? RuleId { get; set; }

    [JsonPropertyName("activityId")]
    public string? ActivityId { get; set; }

    [JsonPropertyName("requestId")]
    public string? RequestId { get; set; }

    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("token")]
    public string? Token { get; set; }

    [JsonPropertyName("taskId")]
    public string? TaskId { get; set; }
}

public class UniqueIdentifierLandData
{
    // This field appears in the official sample body inside landData,
    // but it is not listed as a required landData field in the parameter table.
    [JsonPropertyName("mapConfirmationTrackingCode")]
    public string? MapConfirmationTrackingCode { get; set; }

    [JsonPropertyName("uniqueIdentifier")]
    public string? UniqueIdentifier { get; set; }

    [JsonPropertyName("unitBlockNumber")]
    public string? UnitBlockNumber { get; set; }

    [JsonPropertyName("unitNumber")]
    public string? UnitNumber { get; set; }

    [JsonPropertyName("warehouseNumberList")]
    public List<decimal>? WarehouseNumberList { get; set; }

    [JsonPropertyName("parkingNumberList")]
    public List<decimal>? ParkingNumberList { get; set; }

    [JsonPropertyName("otherAttachmentsList")]
    public List<UniqueIdentifierAttachment>? OtherAttachmentsList { get; set; }
}

public class UniqueIdentifierAttachment
{
    [JsonPropertyName("attachmentsType")]
    public string? AttachmentsType { get; set; }

    [JsonPropertyName("attachmentsNumber")]
    public decimal? AttachmentsNumber { get; set; }
}

public class UniqueIdentifierUserInfo
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("family")]
    public string? Family { get; set; }

    [JsonPropertyName("usertype")]
    public string? UserType { get; set; }

    [JsonPropertyName("nationalCode")]
    public string? NationalCode { get; set; }

    [JsonPropertyName("nationalId")]
    public string? NationalId { get; set; }

    [JsonPropertyName("totalShare")]
    public decimal? TotalShare { get; set; }

    [JsonPropertyName("shareOf")]
    public decimal? ShareOf { get; set; }

    [JsonPropertyName("landArseh")]
    public decimal? LandArseh { get; set; }

    [JsonPropertyName("landAyan")]
    public decimal? LandAyan { get; set; }
}

public class UniqueIdentifierResponseEnvelope
{
    [JsonPropertyName("status")]
    public List<UniqueIdentifierResponseStatus> Status { get; set; } = new();

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public UniqueIdentifierResponseData? Data { get; set; }
}

public class UniqueIdentifierResponseStatus
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

public class UniqueIdentifierResponseData
{
    [JsonPropertyName("msbTrackingCode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? MsbTrackingCode { get; set; }

    [JsonPropertyName("timestamp")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? Timestamp { get; set; }
}
