using CompanionAPI.Contracts.ExerciseContracts;

namespace CompanionAPI.Contracts.OnboardingContracts;

public record UserOnboardingRequest(
    string Name,
    string Email,
    string Gender,
    int Age,
    int Height,
    int Weight,
    List<ExerciseRequest>? Exercises
);
