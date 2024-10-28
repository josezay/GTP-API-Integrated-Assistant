using CompanionAPI.Contracts.AIContracts;
using CompanionAPI.Contracts.GoalContracts;
using CompanionAPI.Contracts.ReportContracts;
using CompanionAPI.Services.AiService;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CompanionAPI.Controllers;

[Route("api/[controller]")]
public class AIController : ApiController
{
    private readonly IMapper _mapper;
    private readonly IAIService _aiService;

    public AIController(
        IAIService aiService,
        IMapper mapper
        )
    {
        _aiService = aiService;
        _mapper = mapper;
    }

    [HttpPost("create-assistant")]
    [SwaggerOperation(Summary = "Create a new assistant", Description = "Creates a new assistant with the provided details.")]
    [SwaggerResponse(200, "Assistant created successfully")]
    [SwaggerResponse(400, "Invalid input data")]
    public IActionResult CreateAssistant()
    {
        var assistantResponse =  _aiService.CreateAssistant();

        return assistantResponse.Match(
            goalResult => Ok(_mapper.Map<CreateAssistantResponse>(goalResult)),
            errors => Problem(errors));
    }
}
