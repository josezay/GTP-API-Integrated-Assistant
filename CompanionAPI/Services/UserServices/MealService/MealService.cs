using CompanionAPI.Contracts.MealContracts;
using CompanionAPI.Entities;
using CompanionAPI.Errors;
using CompanionAPI.Repositories.UserRepository;
using CompanionAPI.Services.UserServices.GoalService;
using ErrorOr;

namespace CompanionAPI.Services.UserServices.MealService;

public class MealService : IMealService
{
    private readonly IUserRepository _userRepository;
    public MealService(
            IUserRepository userRepository
        )
    {
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<Meal>> AddMeal(string userId, AddMealRequest request)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null)
        {
            return UserErrors.UserNotExists;
        }

        var meal = Meal.Create(request.Name, request.Calories, request.Proteins, request.Quantity, request.Unit);


        user.AddMeal(meal);

        await _userRepository.UpdateUserAsync(user);

        return meal;
    }
}