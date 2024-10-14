using CompanionAPI.Requests;
using ErrorOr;

namespace CompanionAPI.Interfaces;

public interface IOnboardService
{
    public Task<ErrorOr<Success>> OnboardUser(UserOnboardingRequest request, CancellationToken cancellationToken);
}
