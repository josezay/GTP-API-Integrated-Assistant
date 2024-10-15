﻿using CompanionAPI.Contracts.UserOnboardingContracts;
using CompanionAPI.Models;
using ErrorOr;

namespace CompanionAPI.Interfaces;

public interface IOnboardService
{
    Task<ErrorOr<User>> OnboardUser(UserOnboardingRequest request, CancellationToken cancellationToken);
}