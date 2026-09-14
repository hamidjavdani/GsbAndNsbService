using GSB.Test.Api.Models.Requests;
using GSB.Test.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GSB.Test.Api.Controllers;

[ApiController]
[Route("api/msb")]
public class MsbController : ControllerBase
{
    private readonly IMsbService _msbService;

    public MsbController(IMsbService msbService)
    {
        _msbService = msbService;
    }

    [HttpPost("document-verification-inquiry")]
    public async Task<IActionResult> DocumentVerificationInquiry([FromBody] G2GInquiryRequest request)
    {
        try
        {
            var result = await _msbService.G2GInquiryAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
    }
}
