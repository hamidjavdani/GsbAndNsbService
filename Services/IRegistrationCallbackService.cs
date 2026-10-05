using GSB.Test.Api.Models.Callback;

namespace GSB.Test.Api.Services;

public interface IRegistrationCallbackService
{
    /// <summary>
    /// ذخیره Callback اصالت سند دریافتی از پنجره واحد
    /// </summary>
    Task<bool> SaveCallbackAsync(RegistrationCallbackRequest request);

    /// <summary>
    /// ذخیره پاسخ وضعیت ثبت ماده 14 دریافتی از MSB
    /// </summary>
    Task<bool> SaveRegistrationStatusCallbackAsync(
        RegistrationStatusCallbackRequest request);
}
