using CompanionAPI.Interfaces;
using CompanionAPI.Requests;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CompanionAPI.Controllers;

[Route("api/[controller]")]
public class OnboardingController : ApiController
{
    private readonly IOnboardService _onboardService;

    public OnboardingController(IOnboardService onboardService)
    {
        _onboardService = onboardService;
    }

    [HttpPost]
    [Route("onboard")]
    [SwaggerOperation(Summary = "Register a new user", Description = "Registers a new user with the provided details.")]
    [SwaggerResponse(200, "User registered successfully")]
    [SwaggerResponse(400, "Invalid input data")]
    public async Task<IActionResult> OnboardUser([FromBody] UserOnboardingRequest request)
    {
        await _onboardService.OnboardUser(request, HttpContext.RequestAborted);
        return Ok("User registered successfully");
    }
}
