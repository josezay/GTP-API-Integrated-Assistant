using CompanionAPI.Contracts.GoalContracts;
using CompanionAPI.Entities;
using CompanionAPI.Errors;
using CompanionAPI.Repositories.UserRepository;
using ErrorOr;
using MapsterMapper;

namespace CompanionAPI.Services.UserServices.GoalService;

public class GoalService : IGoalService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GoalService(
            IUserRepository userRepository,
            IMapper mapper
        )
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public ErrorOr<Goal> CalcGoal(User user)
    {
        //TODO: Pedrão, calcular (ou chamar quem calcule) o TMB aqui
        return Goal.Create(2000, 100);
    }

    public async Task<ErrorOr<Goal>> AddGoalToUser(AddGoalRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.UserId);
        if (user is null)
        {
            return UserErrors.UserNotExists;
        }

        var goal = Goal.Create(request.Calories, request.Proteins);

        user.AddGoal(goal);

        await _userRepository.UpdateUserAsync(user);

        return goal;
    }

    public async Task<ErrorOr<UserSumaryResponse>> GetUserSummary(string userId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null)
        {
            return UserErrors.UserNotExists;
        }

        var goal = user.Goals.LastOrDefault();
        
        if (goal is null)
        {
            return UserErrors.UserHasNoGoals;
        }

        var summary = new UserSumaryResponse(
            _mapper.Map<GoalResponse>(user.Goals.LastOrDefault()!),
            user.DailySummary.Where(ds => ds.Date.Date == DateTime.Now.Date).FirstOrDefault()!
        );

        return summary;
    }
}