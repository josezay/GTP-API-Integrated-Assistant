namespace CompanionAPI.Contracts.AIContracts.Dtos;

public record ReportDto(
    string ReportType,
    List<NutrientReportDto>? NutrientReport,
    WeightReportDto? WeightReport,
    List<ActivityReportDto>? ActivityReport,
    AssistantResponseDto? AssistantResponse
);
