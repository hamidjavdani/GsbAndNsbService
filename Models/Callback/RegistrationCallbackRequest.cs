using System.Text.Json.Serialization;

namespace GSB.Test.Api.Models.Callback;

public class RegistrationCallbackRequest
{
    /// <summary>
    /// کد وضعیت Callback
    /// 200 = موفق
    /// 201 = خطا
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; set; }

    /// <summary>
    /// اطلاعات دریافتی از پنجره واحد
    /// </summary>
    [JsonPropertyName("data")]
    public RegistrationCallbackData? Data { get; set; }

    /// <summary>
    /// اطلاعات خطا
    /// </summary>
    [JsonPropertyName("error")]
    public RegistrationCallbackError? Error { get; set; }
}