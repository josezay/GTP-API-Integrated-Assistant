using CompanionAPI.Contracts.UserOnboardingContracts;
using CompanionAPI.Models;
using ErrorOr;

namespace CompanionAPI.Services.OnboardService;

public interface IOnboardService
{
    Task<ErrorOr<User>> OnboardUser(UserOnboardingRequest request, CancellationToken cancellationToken);
}