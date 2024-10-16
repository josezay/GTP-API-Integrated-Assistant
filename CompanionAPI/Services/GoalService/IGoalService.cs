using CompanionAPI.Entities;
using ErrorOr;

namespace CompanionAPI.Services.GoalService;

public interface IGoalService
{
    ErrorOr<Goal> CalcGoal(User user);
}