using CompanionAPI.Common.Errors;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace CompanionAPI;

public static class DependencyInjection
{
    public static IServiceCollection ProjectStartup(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddSingleton<ProblemDetailsFactory, CompanionProblemDetailsFactory>();

        return services;
    }
}
