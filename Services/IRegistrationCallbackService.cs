

using GSB.Test.Api.Models.Callback;

namespace GSB.Test.Api.Services;

public interface IRegistrationCallbackService
{
    /// <summary>
    /// ذخیره Callback دریافتی از پنجره واحد
    /// </summary>
    Task<bool> SaveCallbackAsync(RegistrationCallbackRequest request);
}