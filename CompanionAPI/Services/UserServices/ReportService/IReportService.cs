using CompanionAPI.Contracts.ReportContracts;
using ErrorOr;

namespace CompanionAPI.Services.UserServices.ReportService;

public interface IReportService
{
    Task<ErrorOr<AddReportResponse>> AddReport(AddReportRequest request);
}