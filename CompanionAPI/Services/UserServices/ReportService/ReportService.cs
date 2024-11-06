using CompanionAPI.Contracts.AIContracts.Dtos;
using CompanionAPI.Contracts.ReportContracts;
using CompanionAPI.Entities;
using CompanionAPI.Errors;
using CompanionAPI.Repositories.UserRepository;
using CompanionAPI.Services.AiService;
using CompanionAPI.Services.UserServices.GoalService;
using ErrorOr;

namespace CompanionAPI.Services.UserServices.ReportService;

public class ReportService : IReportService
{
    private readonly IUserRepository _userRepository;
    private readonly IAIService _openAiService;
    private readonly IGoalService _goalService;

    public ReportService(
            IUserRepository userRepository,
            IAIService openAiService,
            IGoalService goalService
        )
    {
        _userRepository = userRepository;
        _openAiService = openAiService;
        _goalService = goalService;
    }

    public async Task<ErrorOr<AddReportResponse>> AddReport(AddReportRequest request)
    {
        var userResult = await GetUserByIdAsync(request.UserId);
        if (userResult.IsError)
        {
            return userResult.Errors;
        }

        var user = userResult.Value;
        var report = CreateAndAddReport(user, request.Query);

        var aiResponse = await _openAiService.CallAI(user, report.Query);
        if (aiResponse.IsError)
        {
            return aiResponse.Errors;
        }

        var addReportResponse = ProcessAIResponse(user, aiResponse.Value);

        await _userRepository.UpdateUserAsync(user);

        return addReportResponse;
    }

    private async Task<ErrorOr<User>> GetUserByIdAsync(string userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null)
        {
            return UserErrors.UserNotExists;
        }
        return user;
    }

    private Report CreateAndAddReport(User user, string query)
    {
        var report = Report.Create(query);
        user.AddReport(report);
        return report;
    }

    private ErrorOr<AddReportResponse> ProcessAIResponse(User user, object aiResponse)
    {
        List<AddReportMealResponse> mealResponse = [];
        AddReportGoalResponse? goalResponse = null;

        if (aiResponse is List<object> responseList)
        {
            foreach (var item in responseList)
            {
                if (item is MealDto mealDto)
                {
                    var createdMeal = Meal.Create(
                        name: mealDto.Name,
                        quantity: mealDto.Quantity,
                        calories: mealDto.Calories,
                        proteins: mealDto.Proteins,
                        unit: mealDto.Unit);

                    user.AddMeal(createdMeal);
                    mealResponse.Add(CreateMealResponse(createdMeal));
                }
                else if (item is WeightDto weightDto)
                {
                    if(weightDto.weight > 0)
                    {
                        user.UpdateWeight(weightDto.weight ?? 0);
                        var goal = _goalService.CalcGoal(user);

                        if (goal.IsError)
                        {
                            return goal.Errors;
                        }

                        user.AddGoal(goal.Value);

                        goalResponse = CreateGoalResponse(goal.Value);
                    }
                }
            }
        }

        return new AddReportResponse(mealResponse, goalResponse);
    }

    private AddReportMealResponse CreateMealResponse(Meal meal)
    {
        return new AddReportMealResponse(meal.Name, (int)meal.Calories, (int)meal.Proteins);
    }

    private AddReportGoalResponse CreateGoalResponse(Goal goal)
    {
        return new AddReportGoalResponse(goal.Calories, goal.Proteins);
    }
}