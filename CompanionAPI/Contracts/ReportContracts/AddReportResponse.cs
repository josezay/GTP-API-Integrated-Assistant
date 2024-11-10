namespace CompanionAPI.Contracts.ReportContracts;

public record AddReportResponse(
    List<AddReportMealResponse> Meals,
    AddReportGoalResponse? Goal,
    List<AddReportActivityResponse> Activities
);
