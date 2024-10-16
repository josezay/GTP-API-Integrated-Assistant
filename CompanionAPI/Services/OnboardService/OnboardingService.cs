using CompanionAPI.Contracts.UserOnboardingContracts;
using CompanionAPI.Models;
using CompanionAPI.Repositories.UserRepository;
using ErrorOr;

namespace CompanionAPI.Services.OnboardService;

public class OnboardingService : IOnboardService
{
    private readonly IGoalService _goalService;
    private readonly IUserRepository _userRepository;

    public OnboardingService(
        IGoalService goalService,
        IUserRepository userRepository)
    {
        _goalService = goalService;
        _userRepository = userRepository;
    }
    public async Task<ErrorOr<User>> OnboardUser(UserOnboardingRequest request, CancellationToken cancellationToken)
    {
        var user = User.Onboard(
                request.Name,
                request.Email,
                request.Gender,
                request.Age,
                request.Height,
                request.Weight
            );

        var errorOrGoal = _goalService.CalcGoal();
        if (errorOrGoal.IsError)
        {
            throw new Exception("Failed to calculate goal"); // TODO: Sentry and ErrorOr response
        }

        var goal = errorOrGoal.Value;

        user.AddGoal(goal);

        await _userRepository.SaveUserAsync(user);

        return user;
    }
}