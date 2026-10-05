using GSB.Test.Api.Models.Requests;
using GSB.Test.Api.Models.Responses;

namespace GSB.Test.Api.Services;

public interface IMsbService
{
    Task<G2GInquiryResponse> G2GInquiryAsync(G2GInquiryRequest request);
}
