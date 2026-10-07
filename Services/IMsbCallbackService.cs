using GSB.Test.Api.Models.Callback;

namespace GSB.Test.Api.Services;

public interface IMsbCallbackService
{
    Task<bool> SaveCallbackAsync(MsbCallbackRequest request);
}
