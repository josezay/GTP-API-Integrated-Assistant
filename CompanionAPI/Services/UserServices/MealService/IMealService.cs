using CompanionAPI.Contracts.MealContracts;
using CompanionAPI.Entities;
using ErrorOr;

namespace CompanionAPI.Services.UserServices.GoalService;

public interface IMealService
{
    Task<ErrorOr<Meal>> AddMeal(string userId, AddMealRequest request);
}