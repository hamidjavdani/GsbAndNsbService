using GSB.Test.Api.Models.Callback;

namespace GSB.Test.Api.Services;

public interface IUniqueIdentifierCallbackService
{
    Task<bool> SaveAsync(UniqueIdentifierResponseRequest request, string rawJson);
}
