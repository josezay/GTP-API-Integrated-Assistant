using CompanionAPI.Contracts.ReportContracts;
using CompanionAPI.Entities;
using CompanionAPI.Errors;
using CompanionAPI.Repositories.UserRepository;
using CompanionAPI.Services.AiService;
using ErrorOr;

namespace CompanionAPI.Services.UserServices.ReportService;

public class ReportService : IReportService
{
    private readonly IUserRepository _userRepository;
    private readonly IAIService _openAiService;

    public ReportService(
            IUserRepository userRepository,
            IAIService openAiService
        )
    {
        _userRepository = userRepository;
        _openAiService = openAiService;
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

        var aiResponse = await _openAiService.CallAI(report.Query);
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

    private AddReportResponse ProcessAIResponse(User user, object aiResponse)
    {
        AddReportMealResponse? mealResponse = null;
        AddReportGoalResponse? goalResponse = null;

        if (aiResponse is List<object> responseList)
        {
            foreach (var item in responseList)
            {
                if (item is Meal meal)
                {
                    user.AddMeal(meal);
                    mealResponse = CreateMealResponse(meal);
                }
                else if (item is Goal goal)
                {
                    user.AddGoal(goal);
                    goalResponse = CreateGoalResponse(goal);
                }
            }
        }

        return new AddReportResponse(mealResponse, goalResponse);
    }

    private AddReportMealResponse CreateMealResponse(Meal meal)
    {
        return new AddReportMealResponse((int)meal.Calories, (int)meal.Proteins);
    }

    private AddReportGoalResponse CreateGoalResponse(Goal goal)
    {
        return new AddReportGoalResponse(goal.Calories, goal.Proteins);
    }
}