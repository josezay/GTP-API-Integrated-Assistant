using CompanionAPI.Contracts.AIContracts;
using ErrorOr;

namespace CompanionAPI.Services.AiService;

public interface IAIService
{
    ErrorOr<CreateAssistantResponse> CreateAssistant();
    Task<ErrorOr<string>> CallAI(string userId, string message);
}