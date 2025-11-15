<<<<<<< HEAD
using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FlightWatch.Api.HealthChecks;

public class RabbitMqHealthCheck(IBusControl busControl) : IHealthCheck
{
    private readonly IBusControl _busControl = busControl;

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (_busControl != null)
            {
                return Task.FromResult(
                    HealthCheckResult.Healthy("RabbitMQ connection is healthy"));
            }

            return Task.FromResult(
                HealthCheckResult.Degraded("RabbitMQ bus control is null"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(
                HealthCheckResult.Unhealthy(
                    "RabbitMQ connection failed",
                    exception: ex));
        }
    }
}

=======
using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FlightWatch.Api.HealthChecks;

public class RabbitMqHealthCheck(IBusControl busControl) : IHealthCheck
{
    private readonly IBusControl _busControl = busControl;

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (_busControl != null)
            {
                return Task.FromResult(
                    HealthCheckResult.Healthy("RabbitMQ connection is healthy"));
            }

            return Task.FromResult(
                HealthCheckResult.Degraded("RabbitMQ bus control is null"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(
                HealthCheckResult.Unhealthy(
                    "RabbitMQ connection failed",
                    exception: ex));
        }
    }
}

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
