namespace CompanionAPI.Contracts.AIContracts.Dtos;

public record ActivityReportDto(
    string Name,
    int DurationInMinutes,
    double CaloriesBurned
);
