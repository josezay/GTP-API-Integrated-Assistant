using CompanionAPI.Contracts.GoalContracts;
using CompanionAPI.Services.UserServices.GoalService;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CompanionAPI.Controllers;

[Route("api/[controller]")]
public class DashBoardController: ApiController
{
    private readonly IMapper _mapper;
    private readonly IGoalService _goalService;

    public DashBoardController(
        IGoalService goalService,
        IMapper mapper
        )
    {
        _goalService = goalService;
        _mapper = mapper;
    }

    [HttpGet("summary")]
    [SwaggerOperation(Summary = "Get user summary", Description = "Fetches the overall summary of the user with various indicators.")]
    [SwaggerResponse(200, "Summary fetched successfully")]
    [SwaggerResponse(404, "User not found")]
    public async Task<IActionResult> GetUserSummary([FromQuery] string userId)
    {
        var summaryResult = await _goalService.GetUserSummary(userId, HttpContext.RequestAborted);

        return summaryResult.Match(
            summary => Ok(summary),
            errors => Problem(errors));
    }
}
