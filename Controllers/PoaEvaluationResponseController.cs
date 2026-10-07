using GSB.Test.Api.Models.Callback;
using GSB.Test.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace GSB.Test.Api.Controllers;

[ApiController]
public class PoaEvaluationResponseController : ControllerBase
{
    private readonly IPoaEvaluationCallbackService _service;
    private readonly IConfiguration _configuration;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public PoaEvaluationResponseController(
        IPoaEvaluationCallbackService service,
        IConfiguration configuration)
    {
        _service = service;
        _configuration = configuration;
    }

    [HttpPost("/interagency-inquiry/poa-evaluation-response/v1/create")]
    public async Task<IActionResult> Create()
    {
        if (!ValidateApiKey())
        {
            return Unauthorized(CreateErrorAck(
                "UNAUTHORIZED",
                "کلید دسترسی معتبر نیست."));
        }

        var rawJson = await ReadRawBodyAsync();
        if (string.IsNullOrWhiteSpace(rawJson))
        {
            return BadRequest(CreateErrorAck(
                "INVALID_DATA",
                "بدنه درخواست الزامی است."));
        }

        PoaEvaluationResponseRequest? request;
        try
        {
            request = JsonSerializer.Deserialize<PoaEvaluationResponseRequest>(
                rawJson,
                JsonOptions);
        }
        catch (JsonException)
        {
            return BadRequest(CreateErrorAck(
                "INVALID_DATA",
                "بدنه درخواست باید JSON معتبر باشد."));
        }

        if (request is null ||
            string.IsNullOrWhiteSpace(request.OrganId) ||
            string.IsNullOrWhiteSpace(request.OwTrakingCode) ||
            request.Code is null)
        {
            return BadRequest(CreateErrorAck(
                "INVALID_DATA",
                "organId، owTrakingCode و code الزامی هستند."));
        }

        if (request.Code == 200 && request.Data is null)
        {
            return BadRequest(CreateErrorAck(
                "INVALID_DATA",
                "برای کد 200 فیلد data الزامی است."));
        }

        if (request.Code == 201 && request.Error is null)
        {
            return BadRequest(CreateErrorAck(
                "INVALID_DATA",
                "برای کد 201 فیلد error الزامی است."));
        }

        if (request.Code is not (200 or 201))
        {
            return BadRequest(CreateErrorAck(
                "INVALID_DATA",
                "مقدار code باید 200 یا 201 باشد."));
        }

        var saved = await _service.SaveAsync(request, rawJson);
        if (!saved)
        {
            return StatusCode(500, CreateErrorAck(
                "PROCESSING_ERROR",
                "ذخیره پاسخ ناموفق بود."));
        }

        return Ok(new
        {
            msbTrackingCode = request.OwTrakingCode,
            code = "200",
            message = "OK",
            description = "پردازش درخواست موفق",
            timestamp = GetPersianTimestamp()
        });
    }

    private bool ValidateApiKey()
    {
        var headerName = _configuration["MSB:ApiKeyHeaderName"] ?? "X-MSB-Api-Key";
        var apiKey = Request.Headers[headerName].FirstOrDefault();
        var expectedApiKey = _configuration["MSB:ApiKey"];

        return !string.IsNullOrWhiteSpace(apiKey) &&
               !string.IsNullOrWhiteSpace(expectedApiKey) &&
               apiKey == expectedApiKey;
    }

    private async Task<string> ReadRawBodyAsync()
    {
        using var reader = new StreamReader(Request.Body, Encoding.UTF8);
        return await reader.ReadToEndAsync();
    }

    private static object CreateErrorAck(
        string message,
        string description) => new
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
