using CompanionAPI.Contracts.GoalContracts;
using CompanionAPI.Contracts.ReportContracts;
using CompanionAPI.Services.ReportService;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CompanionAPI.Controllers;

[Route("api/[controller]")]
public class ReportController : ApiController
{
    private readonly IMapper _mapper;
    private readonly IReportService _reportService;

    public ReportController(
        IReportService reportService,
        IMapper mapper
        )
    {
        _reportService = reportService;
        _mapper = mapper;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new report", Description = "Creates a new report with the provided details.")]
    [SwaggerResponse(200, "Report created successfully")]
    [SwaggerResponse(400, "Invalid input data")]
    public async Task<IActionResult> CreateReport([FromBody] AddReportRequest request)
    {
        var goalResult = await _reportService.AddReport(request);

        return goalResult.Match(
            goalResult => Ok(_mapper.Map<GoalResponse>(goalResult)),
            errors => Problem(errors));
    }
}
