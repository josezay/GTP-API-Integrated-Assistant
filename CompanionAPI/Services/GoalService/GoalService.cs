using CompanionAPI.Entities;
using ErrorOr;

namespace CompanionAPI.Services.GoalService;

public class GoalService : IGoalService
{
    public ErrorOr<Goal> CalcGoal(string gender, int age, int height, int weigh) // Unidades de medida: altura em cm, peso em gramas
    {
        return Goal.Create(2000, 100);
    }

}