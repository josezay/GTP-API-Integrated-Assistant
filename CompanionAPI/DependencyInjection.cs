using CompanionAPI.Common.Errors;
using CompanionAPI.Repositories.UserRepository;
using CompanionAPI.Services.AiService;
using CompanionAPI.Services.UserServices.GoalService;
using CompanionAPI.Services.UserServices.MealService;
using CompanionAPI.Services.UserServices.OnboardService;
using CompanionAPI.Services.UserServices.ReportService;
using CompanionAPI.Settings.AppSettings;
using FluentValidation;
using FluentValidation.AspNetCore;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Reflection;

namespace CompanionAPI;

public static class DependencyInjection
{
    public static IServiceCollection ProjectStartup(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddServices()
            .AddServiceDescriptors()
            .AddValidators()
            .AddControllersDeps()
            .AddMappings()
            .AddSwagger()
            .AddConfig(configuration)
            .AddPersistence();

        return services;
    }

    private static IServiceCollection AddControllersDeps(this IServiceCollection services)
    {
        services.AddControllers();

        return services;
    }

    private static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OpenAISettings>(configuration.GetSection("OpenAI"));

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAIService, AIService>();
        services.AddScoped<IGoalService, GoalService>();
        services.AddScoped<IMealService, MealService>();
        services.AddScoped<IOnboardService, OnboardingService>();
        services.AddScoped<IReportService, ReportService>();

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
        string credentialsPath = "Properties/serviceAccountKey.json";

        GoogleCredential credential = GoogleCredential.FromFile(credentialsPath);
        FirestoreClientBuilder clientBuilder = new FirestoreClientBuilder
        {
            Credential = credential
        };
        FirestoreClient client = clientBuilder.Build();
        FirestoreDb db = FirestoreDb.Create("companion-f6b6a", client);
        services.AddSingleton(db);

        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}