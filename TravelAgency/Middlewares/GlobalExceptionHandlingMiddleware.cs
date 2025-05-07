using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using TravelAgency.Exceptions;

namespace TravelAgency.Middlewares;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception ocurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        if (exception is NotFoundException)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
        }
        else if (exception is ConflictException)

        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
        }
        else if (exception is BadRequestException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        
        else if (exception is FeatureNotImplementedException)
        {
            context.Response.StatusCode = StatusCodes.Status501NotImplemented;
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        }
       
        var response = new
        {
            status = "Error",
            message = "An unexpected error occurred.",
            deitaledMessage = exception.Message,
            stackTrace = exception.StackTrace
        };
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }
}

public static class GlobalExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        }
    }
