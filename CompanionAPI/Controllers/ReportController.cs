using CompanionAPI.Contracts.GoalContracts;
using CompanionAPI.Contracts.ReportContracts;
using CompanionAPI.Services.UserServices.ReportService;
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
        var addReportResult = await _reportService.AddReport(request);

        return addReportResult.Match(
            addReportResult => Ok(addReportResult),
            errors => Problem(errors));
    }

    [HttpGet("last-three-days/{userId}")]
    [SwaggerOperation(Summary = "Get reports from the last 3 days", Description = "Retrieves all reports created in the last 3 days for a specific user.")]
    [SwaggerResponse(200, "Reports retrieved successfully")]
    [SwaggerResponse(400, "Invalid request")]
    public async Task<IActionResult> GetReportsFromLastThreeDays(string userId)
    {
        var reportsResult = await _reportService.GetReportsFromLastThreeDays(userId);

        return reportsResult.Match(
            reports => Ok(reports),
            errors => Problem(errors));
    }
}
