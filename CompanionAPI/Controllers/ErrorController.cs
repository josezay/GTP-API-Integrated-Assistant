using Microsoft.AspNetCore.Mvc;

namespace CompanionAPI.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorsController : ApiController
{
    [Route("/error")]
    public IActionResult Error()
    {
        return Problem();
    }
}