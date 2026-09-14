using System.Text.Json.Serialization;

namespace GSB.Test.Api.Models.Callback;

/// <summary>
/// اطلاعات تصدیق اصالت اسناد مالکیت با مشخصات سند
/// مطابق جدول ۲ مستند رسمی نسخه 2.02
/// </summary>
public class ConfirmDocumentInfo
{
    [JsonPropertyName("HasOtherRestriction")]
    public string? HasOtherRestriction { get; set; }

    [JsonPropertyName("HasRahn")]
    public string? HasRahn { get; set; }

    [JsonPropertyName("IsArrested")]
    public string? IsArrested { get; set; }

    [JsonPropertyName("Successful")]
    public string? Successful { get; set; }

    [JsonPropertyName("RequestDateTime")]
    public string? RequestDateTime { get; set; }

    [JsonPropertyName("ResponseDateTime")]
    public string? ResponseDateTime { get; set; }

    [JsonPropertyName("Area")]
    public string? Area { get; set; }

    [JsonPropertyName("DocPrintDate")]
    public string? DocPrintDate { get; set; }

    [JsonPropertyName("EPieceTypeTitle")]
    public string? EPieceTypeTitle { get; set; }

    [JsonPropertyName("EstatePostCode")]
    public string? EstatePostCode { get; set; }

    [JsonPropertyName("EstateTypeTitle")]
    public string? EstateTypeTitle { get; set; }

    [JsonPropertyName("EstateUsingType")]
    public string? EstateUsingType { get; set; }

    [JsonPropertyName("HasEasement")]
    public string? HasEasement { get; set; }

    [JsonPropertyName("JamCode")]
    public string? JamCode { get; set; }

    [JsonPropertyName("OwnerFamily")]
    public string? OwnerFamily { get; set; }

    [JsonPropertyName("OwnerIdentityNo")]
    public string? OwnerIdentityNo { get; set; }

    [JsonPropertyName("OwnerName")]
    public string? OwnerName { get; set; }

    [JsonPropertyName("PrintedDocNo")]
    public string? PrintedDocNo { get; set; }

    [JsonPropertyName("Section")]
    public string? Section { get; set; }

    [JsonPropertyName("SubSection")]
    public string? SubSection { get; set; }

    [JsonPropertyName("TheShareOf")]
    public string? TheShareOf { get; set; }

    [JsonPropertyName("TotalShare")]
    public string? TotalShare { get; set; }
}