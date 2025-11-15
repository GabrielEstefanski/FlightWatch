using FlightWatch.Application.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FlightWatch.Api.HealthChecks;

public class OpenSkyHealthCheck(IOpenSkyClient openSkyClient) : IHealthCheck
{
    private readonly IOpenSkyClient _openSkyClient = openSkyClient;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(5));

            var result = await _openSkyClient.GetFlightsByBoundingBoxAsync(
                minLatitude: 0,
                maxLatitude: 1,
                minLongitude: 0,
                maxLongitude: 1);

            if (result.IsSuccess)
            {
                return HealthCheckResult.Healthy("OpenSky API is accessible and responding");
            }

            return HealthCheckResult.Degraded(
                $"OpenSky API returned error: {result.Error.Message}");
        }
        catch (OperationCanceledException)
        {
            return HealthCheckResult.Degraded(
                "OpenSky API health check timed out (>5s)");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "OpenSky API is not accessible",
                exception: ex);
        }
    }
}

