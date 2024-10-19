using CompanionAPI.Contracts.ReportContracts;
using CompanionAPI.Entities;
using ErrorOr;

namespace CompanionAPI.Services.ReportService;

public interface IOpenAiService
{
    Task<string> CallAIAsync(string message);
}