using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RallyDakar.CustomExceptions;


namespace RallyDakar.Controllers
{
	[ApiController]
	[ApiExplorerSettings(IgnoreApi = true)]
	public class ErrorController : ControllerBase
	{
		private readonly ILogger _logger;

		public ErrorController(ILogger<ErrorController> logger)
		{
			_logger = logger;
		}
   
        [Route("/error")]
        [AllowAnonymous]
        public IActionResult HandleError()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;
            var message = exception?.Message;
        
            if (message != null)
            {
                _logger.LogError(message);
                switch (exception)
                {
                    case EntityNotFoundException:
                        return StatusCode(404, message);
                    case InvalidStateException:
                        return StatusCode(400, message);
                    default:
                        return StatusCode(500, message);
                }
            }
        
            return StatusCode(500, $"[{nameof(ErrorController)}] Internal server error. Please try again later.");
        }
    }
}
