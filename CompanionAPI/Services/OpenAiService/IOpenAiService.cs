using CompanionAPI.Entities;

namespace CompanionAPI.Services.ReportService;

public interface IOpenAiService
{
    string CallAI(string message);
}