using CompanionAPI.Contracts.UserOnboardingContracts;
using CompanionAPI.Entities;
using CompanionAPI.Errors;
using CompanionAPI.Repositories.UserRepository;
using CompanionAPI.Services.GoalService;
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
        var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return UserErrors.UserAlreadyExists;
        }

        var user = User.Onboard(
                request.Name,
                request.Email,
                request.Gender,
                request.Age,
                request.Height,
                request.Weight
            );

        var errorOrGoal = _goalService.CalcGoal(
            request.Gender,
            request.Age,
            request.Height,
            request.Weight);

        if (errorOrGoal.IsError)
        {
            return GoalErrors.CalcError; // TODO: Sentry
        }

        var goal = errorOrGoal.Value;

        user.AddGoal(errorOrGoal.Value);

        await _userRepository.SaveUserAsync(user);

        return user;
    }
}