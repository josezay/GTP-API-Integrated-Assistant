using CompanionAPI.Contracts.ReportContracts;
using CompanionAPI.Entities;
using ErrorOr;

namespace CompanionAPI.Services.ReportService;

public interface IReportService
{
    Task<ErrorOr<Report>> AddReport(AddReportRequest request);
}