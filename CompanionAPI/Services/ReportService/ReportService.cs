using CompanionAPI.Contracts.ReportContracts;
using CompanionAPI.Entities;
using CompanionAPI.Errors;
using CompanionAPI.Repositories.UserRepository;
using ErrorOr;

namespace CompanionAPI.Services.ReportService;

public class ReportService : IReportService
{
    private readonly IUserRepository _userRepository;
    private readonly IOpenAiService _openAiService;

    public ReportService(
            IUserRepository userRepository,
            IOpenAiService openAiService
        )
    {
        _userRepository = userRepository;
        _openAiService = openAiService;
    }

    public async Task<ErrorOr<Report>> AddReport(AddReportRequest request)
    {
        var user = await _userRepository.GetUserByIdAsync(request.UserId);
        if (user is null)
        {
            return UserErrors.UserNotExists;
        }

        var report = Report.Create(request.Query);

        var response = await _openAiService.CallAIAsync(report.Query);


        user.AddReport(report);

        await _userRepository.UpdateUserAsync(user);


        return default!;
    }
}