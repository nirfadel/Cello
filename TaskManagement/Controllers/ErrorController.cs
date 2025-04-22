using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.Controllers
{
    [ApiController]
    [AllowAnonymous]
    public class ErrorController : ControllerBase
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("/error")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult HandleError()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = exceptionDetails?.Error;

            if (exception != null)
            {
                _logger.LogError(exception, "Unhandled exception");
            }

            return Problem(
                title: "An error occurred while processing your request.",
                statusCode: 500);
        }
    }
}
