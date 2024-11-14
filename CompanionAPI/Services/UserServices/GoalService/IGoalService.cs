using CompanionAPI.Contracts.GoalContracts;
using CompanionAPI.Entities;
using ErrorOr;

namespace CompanionAPI.Services.UserServices.GoalService;

public interface IGoalService
{
    ErrorOr<Goal> CalcGoal(User user);
    Task<ErrorOr<Goal>> AddGoalToUser(AddGoalRequest request, CancellationToken cancellationToken);
    Task<ErrorOr<UserSumaryResponse>> GetUserSummary(string userId, CancellationToken cancellationToken);
}