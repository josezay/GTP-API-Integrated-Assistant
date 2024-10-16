using CompanionAPI.Contracts.UserOnboardingContracts;
using CompanionAPI.Entities;
using ErrorOr;
using static Google.Rpc.Context.AttributeContext.Types;

namespace CompanionAPI.Services.GoalService;

public interface IGoalService
{
    ErrorOr<Goal> CalcGoal(User user);
}