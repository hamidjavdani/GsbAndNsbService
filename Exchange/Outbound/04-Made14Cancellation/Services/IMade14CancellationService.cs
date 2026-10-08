using GSB.Test.Api.Models.Requests;
using GSB.Test.Api.Models.Responses;

namespace GSB.Test.Api.Services;

public interface IMade14CancellationService
{
    Task<Made14CancellationResponse> CancelMade14Async(
        Made14CancellationRequest request);
}
