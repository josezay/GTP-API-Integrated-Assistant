namespace CompanionAPI.Contracts.AIContracts.Dtos;

public record MealDto(
    string Name,
    double Calories,
    double Proteins,
    double Quantity,
    string Unit
);
