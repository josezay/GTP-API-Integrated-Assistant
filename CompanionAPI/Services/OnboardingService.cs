using CompanionAPI.Interfaces;
using CompanionAPI.Requests;
using ErrorOr;

namespace CompanionAPI.Services;

public class OnboardingService : IOnboardService
{
    public Task<ErrorOr<Success>> OnboardUser(UserOnboardingRequest request, CancellationToken cancellationToken)
    {
        // Process the onboarding logic here
        Console.WriteLine($"User {request.Name} onboarded successfully.");

        return default!;
    }
}
