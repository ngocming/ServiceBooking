using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace ServiceBooking.Api.Middlewares;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            try
            {
                _logger.LogError(ex, "Unhandled exception occurred: {Message}", ex.Message);
            }
            catch
            {
                // Fallback to Console write if logger provider fails
                Console.Error.WriteLine($"[GlobalExceptionHandler] Unhandled exception: {ex}");
            }

            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var (statusCode, title) = exception switch
        {
            KeyNotFoundException => (HttpStatusCode.NotFound, "Resource Not Found"),
            ArgumentException => (HttpStatusCode.BadRequest, "Bad Request"),
            InvalidOperationException => (HttpStatusCode.Conflict, "Conflict Operation"),
            UnauthorizedAccessException => (HttpStatusCode.Forbidden, "Forbidden Access"),
            _ => (HttpStatusCode.InternalServerError, "Internal Server Error")
        };

        context.Response.StatusCode = (int)statusCode;

        var isDev = _env.IsDevelopment();
        // Hide internal exception details in Production for 500 errors
        var detail = (statusCode != HttpStatusCode.InternalServerError || isDev)
            ? exception.Message
            : "An unexpected error occurred. Please contact the system administrator.";

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        if (isDev)
        {
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
    }
}
