using CompanionAPI.Contracts.UserOnboardingContracts;
using CompanionAPI.Services.OnboardService;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CompanionAPI.Controllers;

[Route("api/[controller]")]
public class OnboardingController : ApiController
{
    private readonly IMapper _mapper;
    private readonly IOnboardService _onboardService;

    public OnboardingController(
        IOnboardService onboardService,
        IMapper mapper
        )
    {
        _onboardService = onboardService;
        _mapper = mapper;
    }

    [HttpPost]
    [Route("onboard")]
    [SwaggerOperation(Summary = "Register a new user", Description = "Registers a new user with the provided details.")]
    [SwaggerResponse(200, "User registered successfully")]
    [SwaggerResponse(400, "Invalid input data")]
    public async Task<IActionResult> OnboardUser([FromBody] UserOnboardingRequest request)
    {
        var userResult = await _onboardService.OnboardUser(request, HttpContext.RequestAborted);

        return userResult.Match(
            userResult => Ok(_mapper.Map<UserOnboardingResponse>(userResult)),
            errors => Problem(errors));
    }
}
