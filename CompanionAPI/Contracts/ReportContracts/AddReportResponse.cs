namespace CompanionAPI.Contracts.ReportContracts;

public record AddReportResponse(
    AddReportMealResponse? Meal,
    AddReportGoalResponse? Goal
);
