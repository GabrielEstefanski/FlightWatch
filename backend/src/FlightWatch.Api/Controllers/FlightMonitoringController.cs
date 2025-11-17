using FlightWatch.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FlightWatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlightMonitoringController(
    IFlightMonitoringService monitoringService,
    ILogger<FlightMonitoringController> logger) : ControllerBase
{
    private readonly IFlightMonitoringService _monitoringService = monitoringService;
    private readonly ILogger<FlightMonitoringController> _logger = logger;

    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult HealthCheck()
    {
        return Ok(new
        {
            service = "FlightMonitoring",
            status = "healthy",
            timestamp = DateTime.UtcNow
        });
    }
}

