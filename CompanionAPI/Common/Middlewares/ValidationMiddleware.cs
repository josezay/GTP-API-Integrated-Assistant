using CompanionAPI.Common.Http;
using ErrorOr;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text.Json;

public class ValidationMiddleware
{
    private readonly RequestDelegate _next;

    public ValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint is null)
        {
            await _next(context);
            return;
        }

        var requestType = GetRequestType(endpoint);
        if (requestType is null)
        {
            await _next(context);
            return;
        }

        var requestBody = await ReadRequestBodyAsync(context);
        var request = DeserializeRequest(requestBody, requestType);
        if (request is null)
        {
            await _next(context);
            return;
        }

        var validationResult = await ValidateRequestAsync(context, request, requestType);
        if (!validationResult.IsValid)
        {
            await HandleValidationErrorsAsync(context, validationResult.Errors);
            return;
        }

        await _next(context);
    }

    private static Type? GetRequestType(Endpoint endpoint)
    {
        return endpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>()?.Parameters[0].ParameterType;
    }

    private static async Task<string> ReadRequestBodyAsync(HttpContext context)
    {
        context.Request.EnableBuffering();
        var bodyAsText = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0;
        return bodyAsText;
    }

    private static object? DeserializeRequest(string requestBody, Type requestType)
    {
        return JsonSerializer.Deserialize(requestBody, requestType);
    }

    private static async Task<FluentValidation.Results.ValidationResult> ValidateRequestAsync(HttpContext context, object request, Type requestType)
    {
        var validator = context.RequestServices.GetService(typeof(IValidator<>).MakeGenericType(requestType)) as IValidator;
        if (validator is not null)
        {
            return await validator.ValidateAsync(new ValidationContext<object>(request));
        }
        return new FluentValidation.Results.ValidationResult();
    }

    private static async Task HandleValidationErrorsAsync(HttpContext context, List<FluentValidation.Results.ValidationFailure> validationFailures)
    {
        var errors = validationFailures.ConvertAll(validationFailure => Error.Validation(
            validationFailure.PropertyName,
            validationFailure.ErrorMessage));

        context.Items[HttpContextItemKeys.Errors] = errors;

        var problemDetailsFactory = context.RequestServices.GetRequiredService<ProblemDetailsFactory>();
        var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
            context,
            new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary(),
            StatusCodes.Status400BadRequest,
            "Validation Error",
            detail: "One or more validation errors occurred.");

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
