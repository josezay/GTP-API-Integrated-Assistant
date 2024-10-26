using CompanionAPI.Contracts.GoalContracts;
using CompanionAPI.Services.UserServices.GoalService;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CompanionAPI.Controllers;

[Route("api/[controller]")]
public class GoalController: ApiController
{
    private readonly IMapper _mapper;
    private readonly IGoalService _goalService;

    public GoalController(
        IGoalService goalService,
        IMapper mapper
        )
    {
        _goalService = goalService;
        _mapper = mapper;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new goal", Description = "Creates a new goal with the provided details.")]
    [SwaggerResponse(200, "Goal created successfully")]
    [SwaggerResponse(400, "Invalid input data")]
    public async Task<IActionResult> CreateGoal([FromBody] AddGoalRequest request)
    {
        var goalResult = await _goalService.AddGoal(request, HttpContext.RequestAborted);

        return goalResult.Match(
            goalResult => Ok(_mapper.Map<GoalResponse>(goalResult)),
            errors => Problem(errors));
    }
}
