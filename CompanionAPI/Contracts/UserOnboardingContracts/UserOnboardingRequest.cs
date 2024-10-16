namespace CompanionAPI.Contracts.UserOnboardingContracts;

public record UserOnboardingRequest(
    string Name,
    string Email,
    string Gender,
    int Age,
    double Height,
    double Weight
);
