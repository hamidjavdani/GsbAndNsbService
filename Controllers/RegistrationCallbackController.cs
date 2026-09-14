using GSB.Test.Api.Models.Callback;
using GSB.Test.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace GSB.Test.Api.Controllers
{
    [ApiController]
    [Route("api/registration")]
    public class RegistrationCallbackController : ControllerBase
    {
        private readonly IRegistrationCallbackService _service;
        private readonly IConfiguration _configuration;

        public RegistrationCallbackController(
            IRegistrationCallbackService service,
            IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }

        [HttpPost("document-inquiry-response")]
        [HttpPost("~/document-ownership-verification/create")]
        public async Task<IActionResult> DocumentInquiryResponse([FromBody] RegistrationCallbackRequest request)
        {
            var headerName = _configuration["MSB:ApiKeyHeaderName"] ?? "X-MSB-Api-Key";
            var apiKey = Request.Headers[headerName].FirstOrDefault();
            var expectedApiKey = _configuration["MSB:ApiKey"] ?? _configuration["CallbackApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(expectedApiKey) || apiKey != expectedApiKey)
                return Unauthorized(CreateErrorAck("INVALID_API_KEY", "کلید دسترسی معتبر نیست."));

            if (string.IsNullOrWhiteSpace(request.OrganId) || string.IsNullOrWhiteSpace(request.OwTrakingCode))
                return BadRequest(CreateErrorAck("INVALID_DATA", "organId و owTrakingCode الزامی هستند."));

            if (request.Code == 200 && request.Data is null)
                return BadRequest(CreateErrorAck("INVALID_DATA", "برای کد 200 فیلد data الزامی است."));

            if (request.Code == 201 && request.Error is null)
                return BadRequest(CreateErrorAck("INVALID_DATA", "برای کد 201 فیلد error الزامی است."));

            var saved = await _service.SaveCallbackAsync(request);
            if (!saved)
                return StatusCode(500, CreateErrorAck("PROCESSING_ERROR", "ذخیره پاسخ ناموفق بود."));

            return Ok(new MsbCallbackAckResponse
            {
                MsbTrackingCode = request.OwTrakingCode,
                Code = "200",
                Message = "OK",
                Description = "پردازش درخواست موفق",
                Timestamp = GetPersianTimestamp()
            });
        }

        private static MsbCallbackAckResponse CreateErrorAck(string message, string description) => new()
        {
            MsbTrackingCode = null,
            Code = "000",
            Message = message,
            Description = description,
            Timestamp = GetPersianTimestamp()
        };

        private static string GetPersianTimestamp()
        {
            var now = DateTime.Now;
            var calendar = new PersianCalendar();
            return $"{calendar.GetYear(now):0000}/{calendar.GetMonth(now):00}/{calendar.GetDayOfMonth(now):00}-{now:HH:mm:ss}";
        }
    }
}
