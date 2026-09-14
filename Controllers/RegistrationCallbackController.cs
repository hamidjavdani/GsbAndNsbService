using GSB.Test.Api.Models.Callback;
using GSB.Test.Api.Services;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> DocumentInquiryResponse(
            [FromBody] RegistrationCallbackRequest request)
        {
            var apiKey = Request.Headers["X-Api-Key"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(apiKey) ||
                apiKey != _configuration["CallbackApiKey"])
            {
                return Unauthorized(new
                {
                    code = 401,
                    msg = "Unauthorized"
                });
            }

            _ = await _service.SaveCallbackAsync(request);

            return Ok(new
            {
                code = 200,
                msg = "OK"
            });
        }
    }
}

