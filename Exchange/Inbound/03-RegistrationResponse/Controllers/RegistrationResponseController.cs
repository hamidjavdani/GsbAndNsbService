using GSB.Test.Api.Models.Callback;
using GSB.Test.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

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
            return Unauthorized(CreateErrorAck(
                "UNAUTHORIZED",
                $"{headerName} is invalid."));
        }

        if (request == null ||
            string.IsNullOrWhiteSpace(request.OrganId) ||
            string.IsNullOrWhiteSpace(request.OwTrakingCode) ||
            request.Status is null ||
            request.Result is null ||
            request.Result.Code is null ||
            string.IsNullOrWhiteSpace(request.Result.Message))
        {
            return BadRequest(CreateErrorAck(
                "INVALID_DATA",
                "organId, owTrakingCode, status, result.code and result.msg are required."));
        }

        var saved = await _service.SaveRegistrationStatusCallbackAsync(request);
        if (!saved)
        {
            return StatusCode(500, CreateErrorAck(
                "PROCESSING_ERROR",
                "ذخیره پاسخ ناموفق بود."));
        }

        return Ok(new
        {
            msbTrackingCode = Guid.NewGuid().ToString("N"),
            code = "200",
            message = "OK",
            description = "پردازش درخواست موفق",
            timestamp = GetPersianTimestamp()
        });
    }

    private static object CreateErrorAck(string message, string description) => new
    {
        msbTrackingCode = (string?)null,
        code = "000",
        message,
        description,
        timestamp = GetPersianTimestamp()
    };

    private static string GetPersianTimestamp()
    {
        var now = DateTime.Now;
        var calendar = new PersianCalendar();
        return $"{calendar.GetYear(now):0000}/{calendar.GetMonth(now):00}/{calendar.GetDayOfMonth(now):00}-{now:HH:mm:ss}";
    }
}
