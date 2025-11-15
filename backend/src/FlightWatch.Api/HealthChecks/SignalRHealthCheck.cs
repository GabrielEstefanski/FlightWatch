<<<<<<< HEAD
using FlightWatch.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FlightWatch.Api.HealthChecks;

public class SignalRHealthCheck(IHubContext<FlightHub> hubContext) : IHealthCheck
{
    private readonly IHubContext<FlightHub> _hubContext = hubContext;

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (_hubContext != null && _hubContext.Clients != null)
            {
                return Task.FromResult(
                    HealthCheckResult.Healthy("SignalR Hub is operational"));
            }

            return Task.FromResult(
                HealthCheckResult.Degraded("SignalR Hub context is null"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(
                HealthCheckResult.Unhealthy(
                    "SignalR Hub is not accessible",
                    exception: ex));
        }
    }
}

=======
using FlightWatch.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FlightWatch.Api.HealthChecks;

public class SignalRHealthCheck(IHubContext<FlightHub> hubContext) : IHealthCheck
{
    private readonly IHubContext<FlightHub> _hubContext = hubContext;

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (_hubContext != null && _hubContext.Clients != null)
            {
                return Task.FromResult(
                    HealthCheckResult.Healthy("SignalR Hub is operational"));
            }

            return Task.FromResult(
                HealthCheckResult.Degraded("SignalR Hub context is null"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(
                HealthCheckResult.Unhealthy(
                    "SignalR Hub is not accessible",
                    exception: ex));
        }
    }
}

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
