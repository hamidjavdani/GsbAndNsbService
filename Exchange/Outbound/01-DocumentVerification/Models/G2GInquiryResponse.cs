using System.Text.Json.Serialization;

namespace GSB.Test.Api.Models.Responses;

public class G2GInquiryResponse
{
    /// <summary>
    /// کد نتیجه عملیات
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; set; }

    /// <summary>
    /// پیام سرویس
    /// </summary>
    [JsonPropertyName("msg")]
    public string Msg { get; set; } = string.Empty;

    /// <summary>
    /// کد رهگیری پنجره واحد زمین
    /// </summary>
    [JsonPropertyName("owTrakingCode")]
    public string OwTrakingCode { get; set; } = string.Empty;
}
