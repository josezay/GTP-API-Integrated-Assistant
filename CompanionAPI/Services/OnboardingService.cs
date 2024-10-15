using CompanionAPI.Contracts.UserOnboardingContracts;
using CompanionAPI.Interfaces;
using CompanionAPI.Models;
using ErrorOr;

namespace CompanionAPI.Services;

public class OnboardingService : IOnboardService
{
    public async Task<ErrorOr<User>> OnboardUser(UserOnboardingRequest request, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        var user = new User
        {
            Name = request.Name
        };

        return user;
    }
}