namespace CompanionAPI.Contracts.UserOnboardingContracts;

public record UserOnboardingRequest(
    string Name,
    string Email,
    string Gender,
    int Age,
    int Height,
    int Weight
);
