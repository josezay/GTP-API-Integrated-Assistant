namespace CompanionAPI.Contracts.MealContracts;

public record AddMealRequest(
    string UserId,
    string Name,
    int Quantity,
    int Calories,
    int Proteins,
    string Unit
);
