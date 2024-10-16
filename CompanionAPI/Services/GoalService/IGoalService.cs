using CompanionAPI.Contracts.UserOnboardingContracts;
using CompanionAPI.Models;
using ErrorOr;

namespace CompanionAPI.Services.OnboardService;

public interface IGoalService
{
    ErrorOr<Goal> CalcGoal();
}