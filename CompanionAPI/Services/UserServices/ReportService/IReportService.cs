using CompanionAPI.Contracts.ReportContracts;
using CompanionAPI.Entities;
using ErrorOr;

namespace CompanionAPI.Services.UserServices.ReportService;

public interface IReportService
{
    Task<ErrorOr<AddReportResponse>> AddReport(AddReportRequest request);
    Task<ErrorOr<List<Report>>> GetReportsFromLastThreeDays(string userId);
}