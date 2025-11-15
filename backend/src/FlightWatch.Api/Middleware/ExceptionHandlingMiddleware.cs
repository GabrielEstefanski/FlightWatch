<<<<<<< HEAD
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using FlightWatch.Application.Common;
using FlightWatch.Domain.Exceptions;

namespace FlightWatch.Api.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        _logger.LogError(
            exception,
            "An unhandled exception occurred. TraceId: {TraceId}, Path: {Path}",
            traceId,
            context.Request.Path);

        var (statusCode, problemDetails) = exception switch
        {
            InvalidFlightSearchException validationEx => (
                HttpStatusCode.BadRequest,
                CreateProblemDetails(
                    "Validation Error",
                    "One or more validation errors occurred",
                    (int)HttpStatusCode.BadRequest,
                    validationEx.Message,
                    context.Request.Path,
                    traceId,
                    new Dictionary<string, object>
                    {
                        ["errorCode"] = "VALIDATION_ERROR",
                        ["origin"] = validationEx.Origin ?? string.Empty,
                        ["destination"] = validationEx.Destination ?? string.Empty
                    }
                )
            ),
            AviationStackException aviationEx => (
                aviationEx.StatusCode.HasValue && aviationEx.StatusCode >= 500
                    ? HttpStatusCode.BadGateway
                    : HttpStatusCode.ServiceUnavailable,
                CreateProblemDetails(
                    "External Service Error",
                    "An error occurred while communicating with external service",
                    aviationEx.StatusCode.HasValue && aviationEx.StatusCode >= 500 
                        ? (int)HttpStatusCode.BadGateway 
                        : (int)HttpStatusCode.ServiceUnavailable,
                    aviationEx.Message,
                    context.Request.Path,
                    traceId,
                    new Dictionary<string, object>
                    {
                        ["errorCode"] = "EXTERNAL_SERVICE_ERROR",
                        ["serviceName"] = aviationEx.ServiceName,
                        ["statusCode"] = aviationEx.StatusCode ?? 0
                    }
                )
            ),
            ExternalServiceException externalEx => (
                HttpStatusCode.ServiceUnavailable,
                CreateProblemDetails(
                    "External Service Error",
                    "An error occurred while communicating with external service",
                    (int)HttpStatusCode.ServiceUnavailable,
                    externalEx.Message,
                    context.Request.Path,
                    traceId,
                    new Dictionary<string, object>
                    {
                        ["errorCode"] = "EXTERNAL_SERVICE_ERROR",
                        ["serviceName"] = externalEx.ServiceName
                    }
                )
            ),
            FlightWatchException flightWatchEx => (
                HttpStatusCode.InternalServerError,
                CreateProblemDetails(
                    "Application Error",
                    "An application error occurred",
                    (int)HttpStatusCode.InternalServerError,
                    flightWatchEx.Message,
                    context.Request.Path,
                    traceId,
                    new Dictionary<string, object>
                    {
                        ["errorCode"] = "APPLICATION_ERROR"
                    }
                )
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                CreateProblemDetails(
                    "Internal Server Error",
                    "An unexpected error occurred",
                    (int)HttpStatusCode.InternalServerError,
                    "An internal server error occurred. Please try again later.",
                    context.Request.Path,
                    traceId,
                    new Dictionary<string, object>
                    {
                        ["errorCode"] = "INTERNAL_SERVER_ERROR"
                    }
                )
            )
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(problemDetails, options);
        await context.Response.WriteAsync(json);
    }

    private static ProblemDetailsResponse CreateProblemDetails(
        string title,
        string type,
        int status,
        string detail,
        string instance,
        string traceId,
        Dictionary<string, object>? extensions = null)
    {
        return new ProblemDetailsResponse
        {
            Type = type,
            Title = title,
            Status = status,
            Detail = detail,
            Instance = instance,
            TraceId = traceId,
            Timestamp = DateTime.UtcNow,
            Extensions = extensions
        };
    }
}

=======
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using FlightWatch.Application.Common;
using FlightWatch.Domain.Exceptions;

namespace FlightWatch.Api.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        _logger.LogError(
            exception,
            "An unhandled exception occurred. TraceId: {TraceId}, Path: {Path}",
            traceId,
            context.Request.Path);

        var (statusCode, problemDetails) = exception switch
        {
            InvalidFlightSearchException validationEx => (
                HttpStatusCode.BadRequest,
                CreateProblemDetails(
                    "Validation Error",
                    "One or more validation errors occurred",
                    (int)HttpStatusCode.BadRequest,
                    validationEx.Message,
                    context.Request.Path,
                    traceId,
                    new Dictionary<string, object>
                    {
                        ["errorCode"] = "VALIDATION_ERROR",
                        ["origin"] = validationEx.Origin ?? string.Empty,
                        ["destination"] = validationEx.Destination ?? string.Empty
                    }
                )
            ),
            AviationStackException aviationEx => (
                aviationEx.StatusCode.HasValue && aviationEx.StatusCode >= 500
                    ? HttpStatusCode.BadGateway
                    : HttpStatusCode.ServiceUnavailable,
                CreateProblemDetails(
                    "External Service Error",
                    "An error occurred while communicating with external service",
                    aviationEx.StatusCode.HasValue && aviationEx.StatusCode >= 500 
                        ? (int)HttpStatusCode.BadGateway 
                        : (int)HttpStatusCode.ServiceUnavailable,
                    aviationEx.Message,
                    context.Request.Path,
                    traceId,
                    new Dictionary<string, object>
                    {
                        ["errorCode"] = "EXTERNAL_SERVICE_ERROR",
                        ["serviceName"] = aviationEx.ServiceName,
                        ["statusCode"] = aviationEx.StatusCode ?? 0
                    }
                )
            ),
            ExternalServiceException externalEx => (
                HttpStatusCode.ServiceUnavailable,
                CreateProblemDetails(
                    "External Service Error",
                    "An error occurred while communicating with external service",
                    (int)HttpStatusCode.ServiceUnavailable,
                    externalEx.Message,
                    context.Request.Path,
                    traceId,
                    new Dictionary<string, object>
                    {
                        ["errorCode"] = "EXTERNAL_SERVICE_ERROR",
                        ["serviceName"] = externalEx.ServiceName
                    }
                )
            ),
            FlightWatchException flightWatchEx => (
                HttpStatusCode.InternalServerError,
                CreateProblemDetails(
                    "Application Error",
                    "An application error occurred",
                    (int)HttpStatusCode.InternalServerError,
                    flightWatchEx.Message,
                    context.Request.Path,
                    traceId,
                    new Dictionary<string, object>
                    {
                        ["errorCode"] = "APPLICATION_ERROR"
                    }
                )
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                CreateProblemDetails(
                    "Internal Server Error",
                    "An unexpected error occurred",
                    (int)HttpStatusCode.InternalServerError,
                    "An internal server error occurred. Please try again later.",
                    context.Request.Path,
                    traceId,
                    new Dictionary<string, object>
                    {
                        ["errorCode"] = "INTERNAL_SERVER_ERROR"
                    }
                )
            )
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(problemDetails, options);
        await context.Response.WriteAsync(json);
    }

    private static ProblemDetailsResponse CreateProblemDetails(
        string title,
        string type,
        int status,
        string detail,
        string instance,
        string traceId,
        Dictionary<string, object>? extensions = null)
    {
        return new ProblemDetailsResponse
        {
            Type = type,
            Title = title,
            Status = status,
            Detail = detail,
            Instance = instance,
            TraceId = traceId,
            Timestamp = DateTime.UtcNow,
            Extensions = extensions
        };
    }
}

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
