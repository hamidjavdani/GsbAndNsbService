using GSB.Test.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace GSB.Test.Api.Controllers;

[ApiController]
public class UniqueIdentifierResponseController : ControllerBase
{
    private readonly IRawMsbCallbackService _service;
    private readonly IConfiguration _configuration;

    public UniqueIdentifierResponseController(
        IRawMsbCallbackService service,
        IConfiguration configuration)
    {
        _service = service;
        _configuration = configuration;
    }

    [HttpPost("/made14/send-unique-identifier-response/v1/create")]
    public async Task<IActionResult> Create()
    {
        var authError = ValidateApiKey();
        if (authError is not null)
        {
            return authError;
        }

        var rawJson = await ReadAndValidateJsonAsync();
        if (rawJson is null)
        {
            return BadRequest(CreateAck(null, "400", "INVALID_DATA", "بدنه درخواست باید JSON معتبر باشد."));
        }

        var saved = await _service.SaveRawAsync(rawJson);
        if (!saved)
        {
            return StatusCode(500, CreateAck(null, "500", "PROCESSING_ERROR", "ذخیره درخواست ناموفق بود."));
        }

        return Ok(CreateAck(Guid.NewGuid().ToString("N"), "200", "OK", "پردازش درخواست موفق"));
    }

    private IActionResult? ValidateApiKey()
    {
        var headerName = _configuration["CallbackApiKeyHeaderName"] ?? "X-MSB-Api-Key";
        var apiKey = Request.Headers[headerName].FirstOrDefault();
        var expectedApiKey = _configuration["CallbackApiKey"];

        return string.IsNullOrWhiteSpace(apiKey) ||
               string.IsNullOrWhiteSpace(expectedApiKey) ||
               apiKey != expectedApiKey
            ? Unauthorized(CreateAck(null, "401", "UNAUTHORIZED", "X-MSB-Api-Key is invalid."))
            : null;
    }

    private async Task<string?> ReadAndValidateJsonAsync()
    {
        using var reader = new StreamReader(Request.Body, Encoding.UTF8);
        var rawJson = await reader.ReadToEndAsync();

        if (string.IsNullOrWhiteSpace(rawJson))
        {
            return null;
        }

        try
        {
            using var _ = JsonDocument.Parse(rawJson);
            return rawJson;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static object CreateAck(string? trackingCode, string code, string message, string description) => new
    {
        msbTrackingCode = trackingCode,
        code,
        message,
        description
    };
}
