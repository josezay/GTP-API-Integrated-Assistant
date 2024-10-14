using CompanionAPI.Requests;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CompanionAPI.Controllers;

[Route("api/[controller]")]
public class OnboardingController : ApiController
{
    [HttpPost]
    [Route("onboard")]
    [SwaggerOperation(Summary = "Register a new user", Description = "Registers a new user with the provided details.")]
    [SwaggerResponse(200, "User registered successfully")]
    [SwaggerResponse(400, "Invalid input data")]
    public IActionResult OnboardUser([FromBody] UserOnboardingRequest request)
    {


        return Ok("User registered successfully");
    }
}
