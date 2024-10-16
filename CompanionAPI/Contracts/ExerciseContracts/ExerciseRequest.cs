namespace CompanionAPI.Contracts.ExerciseContracts;

public record ExerciseRequest(
    string ActivityName,
    int WeeklyFrequency,
    int DurationInMinutes
);
