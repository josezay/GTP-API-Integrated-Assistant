namespace CompanionAPI.Contracts.AIContracts.Dtos;

public record ReportDto(
    string ReportType,
    NutrientReportDto? NutrientReport,
    WeightReportDto? WeightReport
);
