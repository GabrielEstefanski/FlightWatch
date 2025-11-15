<<<<<<< HEAD
using System.Diagnostics;

namespace FlightWatch.Api.Middleware;

public class CorrelationIdMiddleware(
    RequestDelegate next,
    ILogger<CorrelationIdMiddleware> logger)
{
    private const string CorrelationIdHeaderName = "X-Correlation-ID";
    private const string RequestIdHeaderName = "X-Request-ID";
    
    private readonly RequestDelegate _next = next;
    private readonly ILogger<CorrelationIdMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);
        var requestId = Guid.NewGuid().ToString();

        context.TraceIdentifier = correlationId;
        
        context.Response.OnStarting(() =>
        {
            if (!context.Response.Headers.ContainsKey(CorrelationIdHeaderName))
            {
                context.Response.Headers.Append(CorrelationIdHeaderName, correlationId);
            }
            
            if (!context.Response.Headers.ContainsKey(RequestIdHeaderName))
            {
                context.Response.Headers.Append(RequestIdHeaderName, requestId);
            }
            
            return Task.CompletedTask;
        });

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["RequestId"] = requestId,
            ["RequestPath"] = context.Request.Path,
            ["RequestMethod"] = context.Request.Method
        }))
        {
            if (Activity.Current != null)
            {
                Activity.Current.AddTag("correlation.id", correlationId);
                Activity.Current.AddTag("request.id", requestId);
            }

            _logger.LogInformation(
                "Request started: {Method} {Path} - CorrelationId: {CorrelationId}, RequestId: {RequestId}",
                context.Request.Method,
                context.Request.Path,
                correlationId,
                requestId);

            var stopwatch = Stopwatch.StartNew();
            
            await _next(context);
            
            stopwatch.Stop();

            _logger.LogInformation(
                "Request completed: {Method} {Path} - Status: {StatusCode}, Duration: {Duration}ms, CorrelationId: {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                correlationId);
        }
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationId) 
            && !string.IsNullOrWhiteSpace(correlationId))
        {
            return correlationId.ToString();
        }

        return Activity.Current?.Id ?? Guid.NewGuid().ToString();
    }
}

=======
using System.Diagnostics;

namespace FlightWatch.Api.Middleware;

public class CorrelationIdMiddleware(
    RequestDelegate next,
    ILogger<CorrelationIdMiddleware> logger)
{
    private const string CorrelationIdHeaderName = "X-Correlation-ID";
    private const string RequestIdHeaderName = "X-Request-ID";
    
    private readonly RequestDelegate _next = next;
    private readonly ILogger<CorrelationIdMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);
        var requestId = Guid.NewGuid().ToString();

        context.TraceIdentifier = correlationId;
        
        context.Response.OnStarting(() =>
        {
            if (!context.Response.Headers.ContainsKey(CorrelationIdHeaderName))
            {
                context.Response.Headers.Append(CorrelationIdHeaderName, correlationId);
            }
            
            if (!context.Response.Headers.ContainsKey(RequestIdHeaderName))
            {
                context.Response.Headers.Append(RequestIdHeaderName, requestId);
            }
            
            return Task.CompletedTask;
        });

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["RequestId"] = requestId,
            ["RequestPath"] = context.Request.Path,
            ["RequestMethod"] = context.Request.Method
        }))
        {
            if (Activity.Current != null)
            {
                Activity.Current.AddTag("correlation.id", correlationId);
                Activity.Current.AddTag("request.id", requestId);
            }

            _logger.LogInformation(
                "Request started: {Method} {Path} - CorrelationId: {CorrelationId}, RequestId: {RequestId}",
                context.Request.Method,
                context.Request.Path,
                correlationId,
                requestId);

            var stopwatch = Stopwatch.StartNew();
            
            await _next(context);
            
            stopwatch.Stop();

            _logger.LogInformation(
                "Request completed: {Method} {Path} - Status: {StatusCode}, Duration: {Duration}ms, CorrelationId: {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                correlationId);
        }
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationId) 
            && !string.IsNullOrWhiteSpace(correlationId))
        {
            return correlationId.ToString();
        }

        return Activity.Current?.Id ?? Guid.NewGuid().ToString();
    }
}

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
