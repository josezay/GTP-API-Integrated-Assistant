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
        var validationError = await ValidateRequest(request);
        if (validationError.IsError)
        {
            return validationError.Errors;
        }

        var user = CreateUser(request);

        var errorOrGoal = CalculateGoal(user);
        if (errorOrGoal.IsError)
        {
            return errorOrGoal.Errors;
        }

        user.AddGoal(errorOrGoal.Value);

        await SaveUser(user);

        return user;
    }

    private async Task<ErrorOr<Success>> ValidateRequest(UserOnboardingRequest request)
    {
        var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return UserErrors.UserAlreadyExists;
        }

        // Add more validation logic here if needed

        return new Success();
    }

    private User CreateUser(UserOnboardingRequest request)
    {
        var exercises = request.Exercises?.Select(e => Exercise.Create(
             e.ActivityName,
             e.WeeklyFrequency,
             e.DurationInMinutes)).ToList() ?? [];

        return User.Onboard(
            request.Name,
            request.Email,
            request.Gender,
            request.Age,
            request.Height,
            request.Weight,
            exercises
        );
    }

    private ErrorOr<Goal> CalculateGoal(User user)
    {
        var goal = _goalService.CalcGoal(user);

        if (goal.IsError)
        {
            return GoalErrors.CalcError; // TODO: Sentry
        }

        return goal;
    }

    private async Task SaveUser(User user)
    {
        await _userRepository.SaveUserAsync(user);
    }
}