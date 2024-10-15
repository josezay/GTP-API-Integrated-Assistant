using CompanionAPI.Contracts.UserOnboardingContracts;
using CompanionAPI.Models;
using CompanionAPI.Repositories.UserRepository;
using ErrorOr;

namespace CompanionAPI.Services.OnboardService;

public class OnboardingService : IOnboardService
{
    private readonly IUserRepository _userRepository;

    public OnboardingService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<ErrorOr<User>> OnboardUser(UserOnboardingRequest request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Name = request.Name
        };

        await _userRepository.SaveUserAsync(user);

        return user;
    }
}