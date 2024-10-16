using CompanionAPI.Contracts.OnboardingContracts;
using CompanionAPI.Entities;
using ErrorOr;

namespace CompanionAPI.Services.OnboardService;

public interface IOnboardService
{
    Task<ErrorOr<User>> OnboardUser(UserOnboardingRequest request, CancellationToken cancellationToken);
}