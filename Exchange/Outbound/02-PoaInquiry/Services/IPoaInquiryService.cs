using GSB.Test.Api.Models.Requests;
using GSB.Test.Api.Models.Responses;

namespace GSB.Test.Api.Services;

public interface IPoaInquiryService
{
    Task<PoaInquiryResponse> SendAsync(PoaInquiryRequest request);
}
