namespace CompanionAPI.Contracts.ReportContracts;

public record AddReportMealResponse(
    string Name,
    int Calories,
    int Proteins
);
