using GSB.Test.Api.Models.Requests;
using GSB.Test.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GSB.Test.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GsbController : ControllerBase
{
    private readonly IGsbService _gsbService;

    public GsbController(IGsbService gsbService)
    {
        _gsbService = gsbService;
    }

    [HttpPost("inquiry")]
    public async Task<IActionResult> Inquiry([FromBody] G2GInquiryRequest request)
    {
        try
        {
            var result = await _gsbService.G2GInquiryAsync(request);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Success = false,
                Message = ex.Message
            });
        }
    }
}