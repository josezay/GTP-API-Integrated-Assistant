using CompanionAPI.Contracts.GoalContracts;
using CompanionAPI.Entities;
using ErrorOr;

namespace CompanionAPI.Services.GoalService;

public interface IReportService
{
    Task<ErrorOr<Report>> AddReport(AddReportRequest request);
}