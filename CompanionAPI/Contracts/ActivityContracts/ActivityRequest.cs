namespace CompanionAPI.Contracts.ActivityContracts;

public record ActivityRequest(
    string Name,
    int WeeklyFrequency,
    int DurationInMinutes
);
