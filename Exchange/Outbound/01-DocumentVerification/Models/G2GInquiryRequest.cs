
using System.Text.Json.Serialization;

namespace GSB.Test.Api.Models.Requests;

public class G2GInquiryRequest
{
    /// <summary>
    /// نام متقاضی
    /// </summary>
    [JsonPropertyName("requesterName")]
    public string RequesterName { get; set; } = string.Empty;

    /// <summary>
    /// نام خانوادگی متقاضی
    /// </summary>
    [JsonPropertyName("requesterFamily")]
    public string RequesterFamily { get; set; } = string.Empty;

    /// <summary>
    /// کد ملی متقاضی
    /// </summary>
    [JsonPropertyName("requesterNationalCode")]
    public string RequesterNationalCode { get; set; } = string.Empty;

    /// <summary>
    /// نام استان دفتر درخواست کننده
    /// </summary>
    [JsonPropertyName("requesterOfficProvinceName")]
    public string RequesterOfficProvinceName { get; set; } = string.Empty;

    /// <summary>
    /// کد ملی کارشناس درخواست کننده
    /// </summary>
    [JsonPropertyName("requesterOfficeCode")]
    public string RequesterOfficeCode { get; set; } = string.Empty;

    /// <summary>
    /// نام و نام خانوادگی کارشناس درخواست کننده
    /// </summary>
    [JsonPropertyName("requesterOfficNumber")]
    public string RequesterOfficNumber { get; set; } = string.Empty;

    /// <summary>
    /// RuleId دریافتی از پنجره واحد زمین
    /// </summary>
    [JsonPropertyName("ruleId")]
    public string RuleId { get; set; } = string.Empty;

    /// <summary>
    /// شناسه یکتای درخواست در سازمان
    /// </summary>
    [JsonPropertyName("requestUniqueId")]
    public string RequestUniqueId { get; set; } = string.Empty;

    /// <summary>
    /// شناسه یکتای سند (Electronic Estate Note Number)
    /// </summary>
    [JsonPropertyName("ElectronicEstateNoteNo")]
    public string ElectronicEstateNoteNo { get; set; } = string.Empty;

    /// <summary>
    /// کد ملی مالک
    /// </summary>
    [JsonPropertyName("nationalitycode")]
    public string NationalityCode { get; set; } = string.Empty;
}