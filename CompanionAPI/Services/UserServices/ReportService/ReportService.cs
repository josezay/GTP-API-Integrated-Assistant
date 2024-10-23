using CompanionAPI.Contracts.ReportContracts;
using CompanionAPI.Entities;
using CompanionAPI.Errors;
using CompanionAPI.Repositories.UserRepository;
using CompanionAPI.Services.AiService;
using ErrorOr;

namespace CompanionAPI.Services.UserServices.ReportService;

public class ReportService : IReportService
{
    private readonly IUserRepository _userRepository;
    private readonly IAIService _openAiService;

    public ReportService(
            IUserRepository userRepository,
            IAIService openAiService
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

        var airesponse = await _openAiService.CallAI(user.Id, report.Query);
        if (airesponse.IsError)
        {
            return airesponse.Errors;
        }

        user.AddReport(report);

        await _userRepository.UpdateUserAsync(user);


        return default!;
    }
}