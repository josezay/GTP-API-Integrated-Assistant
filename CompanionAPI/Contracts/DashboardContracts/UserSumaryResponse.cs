using CompanionAPI.Entities;

namespace CompanionAPI.Contracts.GoalContracts;

public record UserSumaryResponse(
    GoalResponse Goal,
    DailySummary? DailySummary
);
