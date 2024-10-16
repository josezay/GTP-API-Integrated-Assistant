using CompanionAPI.Entities;
using ErrorOr;

namespace CompanionAPI.Services.GoalService;

public class GoalService : IGoalService
{
    public ErrorOr<Goal> CalcGoal(User user)
    {
        //TODO: Pedrão, calcular (ou chamar quem calcule) o TMB aqui
        return Goal.Create(2000, 100);
    }
}