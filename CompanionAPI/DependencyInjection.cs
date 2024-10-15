using CompanionAPI.Common.Errors;
using CompanionAPI.Repositories.UserRepository;
using CompanionAPI.Services.OnboardService;
using FluentValidation;
using FluentValidation.AspNetCore;
using Google.Cloud.Firestore;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Reflection;

namespace CompanionAPI;

public static class DependencyInjection
{
    public static IServiceCollection ProjectStartup(this IServiceCollection services)
    {
        services
            .AddServices()
            .AddServiceDescriptors()
            .AddValidators()
            .AddControllersDeps()
            .AddMappings()
            .AddSwagger()
            .AddPersistence();

        return services;
    }

    private static IServiceCollection AddControllersDeps(this IServiceCollection services)
    {
        services.AddControllers();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IOnboardService, OnboardingService>();

        return services;
    }

    private static IServiceCollection AddServiceDescriptors(this IServiceCollection services)
    {
        services.AddSingleton<ProblemDetailsFactory, CompanionProblemDetailsFactory>();

        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddFluentValidationAutoValidation();

        return services;
    }

    private static IServiceCollection AddMappings(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton(config);

        services.AddScoped<IMapper, Mapper>();
        return services;
    }

    private static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        string projectId = "your-project-id";
        FirestoreDb db = FirestoreDb.Create(projectId);
        services.AddSingleton(db);
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}