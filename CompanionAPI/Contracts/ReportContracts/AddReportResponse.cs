namespace CompanionAPI.Contracts.GoalContracts;

public record AddReportResponse(
    AddReportMealResponse? Meal,
    AddReportGoalResponse? Goal
);
