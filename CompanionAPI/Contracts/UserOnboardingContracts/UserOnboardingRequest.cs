namespace CompanionAPI.Contracts.UserOnboardingContracts;

public record UserOnboardingRequest(
    string Name,
    string Gender,
    int Age,
    double Weight,
    double Height,
    string Goal
);
