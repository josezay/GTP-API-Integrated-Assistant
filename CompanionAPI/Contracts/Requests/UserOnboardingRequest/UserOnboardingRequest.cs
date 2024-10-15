namespace CompanionAPI.Contracts.Requests.UserOnboardingRequest;

public record UserOnboardingRequest(
    string Name,
    string Gender,
    int Age,
    double Weight,
    double Height,
    string Goal
);
