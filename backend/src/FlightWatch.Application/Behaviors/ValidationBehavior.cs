<<<<<<< HEAD
using FlightWatch.Application.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace FlightWatch.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(ILogger<ValidationBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        var validationContext = new ValidationContext(request);
        var validationResults = new List<ValidationResult>();

        if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
        {
            var errors = validationResults
                .Select(vr => vr.ErrorMessage ?? "Validation error")
                .ToList();

            _logger.LogWarning(
                "Validation failed for {RequestName}. Errors: {Errors}",
                requestName,
                string.Join(", ", errors));

            if (typeof(TResponse).IsGenericType &&
                typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var errorMessage = string.Join("; ", errors);
                var error = Error.Validation("VALIDATION_FAILED", errorMessage);

                var resultType = typeof(TResponse);
                var failureMethod = resultType.GetMethod("Failure");

                if (failureMethod != null)
                {
                    var result = failureMethod.Invoke(null, [error]);
                    return (TResponse)result!;
                }
            }
        }

        return await next();
    }
}

=======
using FlightWatch.Application.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace FlightWatch.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(ILogger<ValidationBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        var validationContext = new ValidationContext(request);
        var validationResults = new List<ValidationResult>();

        if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
        {
            var errors = validationResults
                .Select(vr => vr.ErrorMessage ?? "Validation error")
                .ToList();

            _logger.LogWarning(
                "Validation failed for {RequestName}. Errors: {Errors}",
                requestName,
                string.Join(", ", errors));

            if (typeof(TResponse).IsGenericType &&
                typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var errorMessage = string.Join("; ", errors);
                var error = Error.Validation("VALIDATION_FAILED", errorMessage);

                var resultType = typeof(TResponse);
                var failureMethod = resultType.GetMethod("Failure");

                if (failureMethod != null)
                {
                    var result = failureMethod.Invoke(null, [error]);
                    return (TResponse)result!;
                }
            }
        }

        return await next();
    }
}

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
