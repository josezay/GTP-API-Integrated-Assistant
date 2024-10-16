namespace CompanionAPI.Contracts.UserOnboardingContracts;

public record UserOnboardingRequest(
    string Name,
    string Email,
    string Gender,
    int Age,
    int Height,
    int Weight,
    List<ExerciseRequest>? Exercises
);

public record ExerciseRequest(
    string ActivityName,
    int WeeklyFrequency,
    int DurationInMinutes
);
