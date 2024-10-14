using CompanionAPI.Common.Http;
using ErrorOr;
using FluentValidation;
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

        // Lê o corpo da requisição
        context.Request.EnableBuffering();
        var bodyAsText = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0;

        // Determina o tipo da requisição
        var requestType = endpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>()?.Parameters[0].ParameterType;
        if (requestType is null)
        {
            await _next(context);
            return;
        }

        // Deserializa o corpo da requisição
        var request = JsonSerializer.Deserialize(bodyAsText, requestType);
        if (request is null)
        {
            await _next(context);
            return;
        }

        // Obtém o validador
        var validator = context.RequestServices.GetService(typeof(IValidator<>).MakeGenericType(requestType)) as IValidator;
        if (validator is not null)
        {
            var validationResult = await validator.ValidateAsync(new ValidationContext<object>(request));
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .ConvertAll(validationFailure => Error.Validation(
                        validationFailure.PropertyName,
                        validationFailure.ErrorMessage));

                context.Items[HttpContextItemKeys.Errors] = errors;

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { errors });
                return;
            }
        }

        await _next(context);
    }
}
