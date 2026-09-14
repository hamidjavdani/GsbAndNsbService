using System.Text.Json;
using System.Text.Json.Serialization;

namespace GSB.Test.Api.Models.Callback;

/// <summary>
/// اطلاعات تصدیق اصالت سند مالکیت با شماره دفتر املاک الکترونیکی
/// مطابق جدول ۱ مستند رسمی نسخه 2.02
/// </summary>
public class ConfirmDocumentByElectronicInfo
{
    [JsonPropertyName("ErrorMessage")]
    public string? ErrorMessage { get; set; }

    [JsonPropertyName("RequestDateTime")]
    public string? RequestDateTime { get; set; }

    [JsonPropertyName("ResponseDateTime")]
    public string? ResponseDateTime { get; set; }

    [JsonPropertyName("ResponseNo")]
    public string? ResponseNo { get; set; }

    [JsonPropertyName("Successful")]
    public bool? Successful { get; set; }

    [JsonPropertyName("Basic")]
    public string? Basic { get; set; }

    [JsonPropertyName("Secondary")]
    public string? Secondary { get; set; }

    [JsonPropertyName("JamCode")]
    public string? JamCode { get; set; }

    [JsonPropertyName("Section")]
    public string? Section { get; set; }

    [JsonPropertyName("SubSection")]
    public string? SubSection { get; set; }

    [JsonPropertyName("OwnerNationalityCode")]
    public string? OwnerNationalityCode { get; set; }

    [JsonPropertyName("OwnerFamily")]
    public string? OwnerFamily { get; set; }

    [JsonPropertyName("OwnerName")]
    public string? OwnerName { get; set; }

    [JsonPropertyName("EstatePostCode")]
    public string? EstatePostCode { get; set; }

    [JsonPropertyName("ProvinceName")]
    public string? ProvinceName { get; set; }

    [JsonPropertyName("ShareOf")]
    public string? ShareOf { get; set; }

    [JsonPropertyName("ShareText")]
    public string? ShareText { get; set; }

    [JsonPropertyName("ShareTotal")]
    public string? ShareTotal { get; set; }

    [JsonPropertyName("UnitName")]
    public string? UnitName { get; set; }

    [JsonPropertyName("Area")]
    public decimal? Area { get; set; }

    [JsonPropertyName("HasOtherRestriction")]
    public bool? HasOtherRestriction { get; set; }

    [JsonPropertyName("HasRahn")]
    public bool? HasRahn { get; set; }

    [JsonPropertyName("IsArrested")]
    public bool? IsArrested { get; set; }

    [JsonPropertyName("OwnerIdentityNo")]
    public string? OwnerIdentityNo { get; set; }

    [JsonPropertyName("OwnershipType")]
    public string? OwnershipType { get; set; }

    [JsonPropertyName("DocPrintDate")]
    public string? DocPrintDate { get; set; }

    [JsonPropertyName("EPieceTypeTitle")]
    public string? EPieceTypeTitle { get; set; }

    [JsonPropertyName("EstateTypeTitle")]
    public string? EstateTypeTitle { get; set; }

    [JsonPropertyName("EstateUsingType")]
    public string? EstateUsingType { get; set; }

    [JsonPropertyName("HasEasement")]
    public bool? HasEasement { get; set; }

    [JsonPropertyName("PrintedDocNo")]
    public string? PrintedDocNo { get; set; }

    [JsonPropertyName("AreaUnit")]
    public string? AreaUnit { get; set; }

    //[JsonPropertyName("Map")]
    //public byte[]? Map { get; set; }


    [JsonPropertyName("Map")]
    public JsonElement? Map { get; set; }

    [JsonPropertyName("Map_Base64String")]
    public string? Map_Base64String { get; set; }

    [JsonPropertyName("TheShareOf")]
    public decimal? TheShareOf { get; set; }

    [JsonPropertyName("TotalShare")]
    public decimal? TotalShare { get; set; }

    [JsonPropertyName("Address")]
    public string? Address { get; set; }

    [JsonPropertyName("ArrestList")]
    public List<object>? ArrestList { get; set; }

    [JsonPropertyName("Block")]
    public string? Block { get; set; }

    [JsonPropertyName("Class")]
    public string? Class { get; set; }

    [JsonPropertyName("Description")]
    public string? Description { get; set; }

    [JsonPropertyName("Direction")]
    public string? Direction { get; set; }

    [JsonPropertyName("EasementRightSummaryList")]
    public List<object>? EasementRightSummaryList { get; set; }

    [JsonPropertyName("EcaseType")]
    public string? EcaseType { get; set; }

    [JsonPropertyName("ElectronicEstateNoteNo")]
    public string? ElectronicEstateNoteNo { get; set; }

    [JsonPropertyName("EpieceType")]
    public string? EpieceType { get; set; }

    [JsonPropertyName("EstateType")]
    public string? EstateType { get; set; }

    [JsonPropertyName("JointList")]
    public List<object>? JointList { get; set; }

    [JsonPropertyName("Mafruz")]
    public string? Mafruz { get; set; }

    [JsonPropertyName("Name")]
    public string? Name { get; set; }

    [JsonPropertyName("NationalityCode")]
    public string? NationalityCode { get; set; }

    [JsonPropertyName("NoteNumber")]
    public string? NoteNumber { get; set; }

    [JsonPropertyName("OwnershipDocumentary")]
    public string? OwnershipDocumentary { get; set; }

    [JsonPropertyName("PageNoteNumber")]
    public string? PageNoteNumber { get; set; }

    [JsonPropertyName("Piece")]
    public string? Piece { get; set; }

    [JsonPropertyName("Province")]
    public string? Province { get; set; }

    [JsonPropertyName("RestrictList")]
    public List<object>? RestrictList { get; set; }

    [JsonPropertyName("SectionID")]
    public string? SectionID { get; set; }

    [JsonPropertyName("SectionSSAACode")]
    public string? SectionSSAACode { get; set; }

    [JsonPropertyName("SellCause")]
    public string? SellCause { get; set; }

    [JsonPropertyName("Sellable")]
    public bool? Sellable { get; set; }

    [JsonPropertyName("SerialDocNO")]
    public string? SerialDocNO { get; set; }

    [JsonPropertyName("Series")]
    public string? Series { get; set; }

    [JsonPropertyName("SpecialStatus")]
    public string? SpecialStatus { get; set; }

    [JsonPropertyName("SubSectionID")]
    public string? SubSectionID { get; set; }

    [JsonPropertyName("SubSectionSSAACode")]
    public string? SubSectionSSAACode { get; set; }

    [JsonPropertyName("TheText")]
    public string? TheText { get; set; }

    [JsonPropertyName("Unit")]
    public string? Unit { get; set; }

    [JsonPropertyName("UnitId")]
    public string? UnitId { get; set; }

    [JsonPropertyName("Year")]
    public string? Year { get; set; }
}