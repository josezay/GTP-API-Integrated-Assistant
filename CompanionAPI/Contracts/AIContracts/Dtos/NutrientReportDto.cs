namespace CompanionAPI.Contracts.AIContracts.Dtos;

public record NutrientReportDto(
    string Name,
    double Quantity,
    string Unit,
    double Calories,
    double Proteins
);