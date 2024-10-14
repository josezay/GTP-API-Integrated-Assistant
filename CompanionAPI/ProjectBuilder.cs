using CompanionAPI.Common.Errors;
using CompanionAPI.Interfaces;
using CompanionAPI.Services;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Reflection;

namespace CompanionAPI;

public static class ProjectBuilder
{

    public static IApplicationBuilder BuildProject(this IApplicationBuilder app)
    {
        app.UseValidationMiddleware();

        app.UseExceptionHandler("/error");

        return app;
    }

    private static IApplicationBuilder UseValidationMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ValidationMiddleware>();
        return app;
    }

}
