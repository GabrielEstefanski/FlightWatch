<<<<<<< HEAD
using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlightWatch.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var activityId = Activity.Current?.Id ?? Guid.NewGuid().ToString();

        _logger.LogInformation(
            "Handling {RequestName} - ActivityId: {ActivityId}",
            requestName,
            activityId);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next(cancellationToken);

            stopwatch.Stop();

            _logger.LogInformation(
                "Handled {RequestName} - ActivityId: {ActivityId}, Duration: {Duration}ms",
                requestName,
                activityId,
                stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "Error handling {RequestName} - ActivityId: {ActivityId}, Duration: {Duration}ms",
                requestName,
                activityId,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}

=======
using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlightWatch.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var activityId = Activity.Current?.Id ?? Guid.NewGuid().ToString();

        _logger.LogInformation(
            "Handling {RequestName} - ActivityId: {ActivityId}",
            requestName,
            activityId);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next(cancellationToken);

            stopwatch.Stop();

            _logger.LogInformation(
                "Handled {RequestName} - ActivityId: {ActivityId}, Duration: {Duration}ms",
                requestName,
                activityId,
                stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "Error handling {RequestName} - ActivityId: {ActivityId}, Duration: {Duration}ms",
                requestName,
                activityId,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
