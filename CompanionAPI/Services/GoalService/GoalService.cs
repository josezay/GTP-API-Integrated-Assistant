using CompanionAPI.Models;
using ErrorOr;

namespace CompanionAPI.Services.OnboardService;

public class GoalService : IGoalService
{
    public ErrorOr<Goal> CalcGoal()
    {
        return Goal.Create(2000, 100);
    }

}