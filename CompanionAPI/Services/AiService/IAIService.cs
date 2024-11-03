using CompanionAPI.Contracts.AIContracts;
using CompanionAPI.Entities;
using ErrorOr;

namespace CompanionAPI.Services.AiService;

public interface IAIService
{
    ErrorOr<CreateAssistantResponse> CreateAssistant();
    Task<ErrorOr<object>> CallAI(User user, string message);
}