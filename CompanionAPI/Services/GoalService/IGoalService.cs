﻿using CompanionAPI.Contracts.GoalContracts;
using CompanionAPI.Entities;
using ErrorOr;

namespace CompanionAPI.Services.GoalService;

public interface IGoalService
{
    ErrorOr<Goal> CalcGoal(User user);
    Task<ErrorOr<Goal>> AddGoal(AddGoalRequest request, CancellationToken cancellationToken);
}