using CompanionAPI.Contracts.GoalContracts;
using CompanionAPI.Entities;
using CompanionAPI.Errors;
using CompanionAPI.Repositories.UserRepository;
using ErrorOr;

namespace CompanionAPI.Services.GoalService;

public class GoalService : IGoalService
{
    private readonly IUserRepository _userRepository;
    public GoalService(
            IUserRepository userRepository
        )
    {
        _userRepository = userRepository;
    }

    public ErrorOr<Goal> CalcGoal(User user)
    {
        //TODO: Pedrão, calcular (ou chamar quem calcule) o TMB aqui
        return Goal.Create(2000, 100);
    }

    public async Task<ErrorOr<Goal>> AddGoal(AddGoalRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.UserId);
        if (user is null)
        {
            return UserErrors.UserNotExists;
        }

        var goal = Goal.Create(request.Calories, request.Proteins);

        user.AddGoal(goal);

        await _userRepository.UpdateUserAsync(user);

        return goal;
    }
}