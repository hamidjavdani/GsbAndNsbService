using GSB.Test.Api.Models.Callback;
using GSB.Test.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GSB.Test.Api.Controllers;

[ApiController]
public class UniqueIdentifierResponseController : ControllerBase
{
    private readonly IUniqueIdentifierCallbackService _service;
    private readonly IConfiguration _configuration;

    public UniqueIdentifierResponseController(
        IUniqueIdentifierCallbackService service,
        IConfiguration configuration)
    {
        _service = service;
        _configuration = configuration;
    }

    [HttpPost("/made14/send-unique-identifier-response/v1/create")]
    public async Task<IActionResult> Create([FromBody] UniqueIdentifierResponseRequest? request)
    {
        if (!ValidateApiKey())
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(CreateResponse(
                103,
                "داده‌های ارسالی نامعتبر است"));
        }

        if (request is null || HasMissingRequiredData(request))
        {
            return BadRequest(CreateResponse(
                104,
                "اطلاعات مورد نیاز کامل نیست"));
        }

        var rawJson = JsonSerializer.Serialize(request);
        var saved = await _service.SaveAsync(request, rawJson);

        if (!saved)
        {
            return StatusCode(500, CreateResponse(
                107,
                "خطای داخلی سرور"));
        }

        return Ok(CreateResponse(
            100,
            "اطلاعات با موفقیت دریافت شد"));
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

    private static bool HasMissingRequiredData(UniqueIdentifierResponseRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.OrganId) ||
            request.Data is null ||
            string.IsNullOrWhiteSpace(request.MapConfirmationTrackingCode) ||
            request.LandData is null ||
            request.LandData.Count == 0 ||
            request.UserInfo is null ||
            request.UserInfo.Count == 0)
        {
            return true;
        }

        var data = request.Data;
        if (string.IsNullOrWhiteSpace(data.ActionId) ||
            string.IsNullOrWhiteSpace(data.RuleId) ||
            string.IsNullOrWhiteSpace(data.ActivityId) ||
            string.IsNullOrWhiteSpace(data.RequestId) ||
            string.IsNullOrWhiteSpace(data.Code) ||
            string.IsNullOrWhiteSpace(data.Token) ||
            string.IsNullOrWhiteSpace(data.TaskId))
        {
            return true;
        }

        foreach (var land in request.LandData)
        {
            if (string.IsNullOrWhiteSpace(land.UniqueIdentifier) ||
                string.IsNullOrWhiteSpace(land.UnitBlockNumber) ||
                string.IsNullOrWhiteSpace(land.UnitNumber) ||
                land.WarehouseNumberList is null ||
                land.ParkingNumberList is null ||
                land.OtherAttachmentsList is null)
            {
                return true;
            }

            foreach (var attachment in land.OtherAttachmentsList)
            {
                if (string.IsNullOrWhiteSpace(attachment.AttachmentsType) ||
                    attachment.AttachmentsNumber is null)
                {
                    return true;
                }
            }
        }

        foreach (var user in request.UserInfo)
        {
            if (string.IsNullOrWhiteSpace(user.Name) ||
                string.IsNullOrWhiteSpace(user.Family) ||
                string.IsNullOrWhiteSpace(user.UserType) ||
                (string.IsNullOrWhiteSpace(user.NationalCode) &&
                 string.IsNullOrWhiteSpace(user.NationalId)) ||
                user.TotalShare is null ||
                user.ShareOf is null ||
                user.LandArseh is null ||
                user.LandAyan is null)
            {
                return true;
            }
        }

        return false;
    }

    private static UniqueIdentifierResponseEnvelope CreateResponse(
        int code,
        string message) => new()
    {
        Status = new List<UniqueIdentifierResponseStatus>
        {
            new()
            {
                Code = code,
                Message = message
            }
        }
    };
}
