using GSB.Test.Api.Models.Callback;
using GSB.Test.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GSB.Test.Api.Controllers;

[ApiController]
public class RegistrationResponseController : ControllerBase
{
    private readonly IRegistrationCallbackService _service;
    private readonly IConfiguration _configuration;

    public RegistrationResponseController(
        IRegistrationCallbackService service,
        IConfiguration configuration)
    {
        _service = service;
        _configuration = configuration;
    }

    [HttpPost("/made-14/registration-response/v1/create")]
    public async Task<IActionResult> Create([FromBody] RegistrationStatusCallbackRequest request)
    {
        var headerName = _configuration["MSB:ApiKeyHeaderName"] ?? "X-MSB-Api-Key";
        var apiKey = Request.Headers[headerName].FirstOrDefault();
        var expectedApiKey = _configuration["MSB:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey) ||
            string.IsNullOrWhiteSpace(expectedApiKey) ||
            apiKey != expectedApiKey)
        {
            return Unauthorized(new
            {
                msbTrackingCode = (string?)null,
                code = "401",
                message = "UNAUTHORIZED",
                description = "X-MSB-Api-Key is invalid."
            });
        }

        if (request == null ||
            string.IsNullOrWhiteSpace(request.OrganId) ||
            string.IsNullOrWhiteSpace(request.OwTrakingCode))
        {
            return BadRequest(new
            {
                msbTrackingCode = (string?)null,
                code = "400",
                message = "INVALID_DATA",
                description = "organId and owTrakingCode are required."
            });
        }

        var saved = await _service.SaveRegistrationStatusCallbackAsync(request);
        if (!saved)
        {
            return StatusCode(500, new
            {
                msbTrackingCode = (string?)null,
                code = "500",
                message = "PROCESSING_ERROR",
                description = "ذخیره پاسخ ناموفق بود."
            });
        }

        return Ok(new
        {
            msbTrackingCode = Guid.NewGuid().ToString("N"),
            code = "200",
            message = "OK",
            description = "پردازش درخواست موفق"
        });
    }
}
