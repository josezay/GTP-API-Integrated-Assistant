namespace CompanionAPI.Contracts.GoalContracts;

public record AddGoalRequest(
    string UserId,
    int Calories,
    int Proteins
);
