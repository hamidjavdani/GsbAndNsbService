using GSB.Test.Api.Models.Callback;

namespace GSB.Test.Api.Services;

public interface IPoaEvaluationCallbackService
{
    Task<bool> SaveAsync(PoaEvaluationResponseRequest request, string rawJson);
}
