using CompanionAPI.Common.Errors;
using CompanionAPI.Interfaces;
using CompanionAPI.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace CompanionAPI;

public static class DependencyInjection
{
    public static IServiceCollection ProjectStartup(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddServices();
        services.AddSingleton<ProblemDetailsFactory, CompanionProblemDetailsFactory>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IOnboardService, OnboardingService>();

        return services;
    }
}
