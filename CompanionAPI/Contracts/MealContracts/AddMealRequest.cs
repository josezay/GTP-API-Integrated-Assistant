namespace CompanionAPI.Contracts.MealContracts;

public record AddMealRequest(
    string UserId,
    string Name,
    double Quantity,
    double Calories,
    double Proteins,
    string Unit
);
