namespace CompanionAPI.Contracts.ReportContracts;

public record AddReportRequest(
    string UserId,
    string Query
);
