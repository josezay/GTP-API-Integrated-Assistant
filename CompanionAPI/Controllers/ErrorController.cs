using Microsoft.AspNetCore.Mvc;

namespace CompanionAPI.Controllers;

/// <summary>
/// The ErrorsController class handles all unhandled exceptions thrown by the application.
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorsController : ApiController
{
    /// <summary>
    /// Handles the HTTP response when an unhandled exception is thrown.
    /// </summary>
    /// <returns>An HTTP 500 status code in the problem+json format.</returns>
    [Route("/error")]
    public IActionResult Error()
    {
        return Problem();
    }
}
