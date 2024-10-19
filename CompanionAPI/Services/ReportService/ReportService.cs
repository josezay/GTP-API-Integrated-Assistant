using CompanionAPI.Contracts.GoalContracts;
using CompanionAPI.Entities;
using CompanionAPI.Errors;
using CompanionAPI.Repositories.UserRepository;
using ErrorOr;

namespace CompanionAPI.Services.GoalService;

public class ReportService : IReportService
{
    private readonly IUserRepository _userRepository;
    public ReportService(
            IUserRepository userRepository
        )
    {
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<Report>> AddReport(AddReportRequest request)
    {
        var user = await _userRepository.GetUserByIdAsync(request.UserId);
        if (user is null)
        {
            return UserErrors.UserNotExists;
        }

        return default!;
    }
}